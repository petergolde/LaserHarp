/*
  Laser Harp monitoring via Arduino.
  
  Monitors the inputs of 8 photoresistors that detect lasers, multiplexed via
  a 74HC5067 IC and read by the Arduino analog input.
  
  Sends output via serial with the following format:
    57600 baud, 8 data bits, 1 stop.
    
    Sends "~" to start, indicating that data follows.
    While monitoring, sends "~" character every second as a heartbeat.
    Sends character "A" - "H" when laser beam is interrupted.
    Sends character "a" - "h" when laser beam is restored.
    
    Sends character "@" to indicate that debugging information is being output. After
    "@" is received, all input should be ignored until a "~" is received.
 */

// Set this to true to enable automatic calibration of the laser at startup.
// Requires that the lasers are powered by the circuit board so they can be turned
// on and off.
const boolean autoCalibration = true;

// Default threshold if autocalibration is off.
const int defaultThreshold = 100;

// These constants won't change.  They're used to give names
// to the pins used:
const int analogInPin = A0;  // Analog input pin that the potentiometer is attached to

const int multiplexBit0Pin = 2;  // Pin that controls bit 0 of the multiplexer
const int multiplexBit1Pin = 3;  // Pin that controls bit 0 of the multiplexer
const int multiplexBit2Pin = 4;  // Pin that controls bit 0 of the multiplexer
const int multiplexBit3Pin = 5;  // Pin that controls bit 0 of the multiplexer

// Pin for status LED.
const int ledPin = 9;

// Pin for turning lasers on/off
const int laserPin = 10;

const int inputCount = 8;  // Number of inputs we are reading. Can be up to 16.

// Thresholds for input on each pin. >= this value is ON, < this value is OFF.
// These are set in the power-on calibration routine, or set to defaultThreshold.
int thresholds[inputCount];

// Values during calibration.
int calibrateMin[inputCount], calibrateMax[inputCount];

// Current values for each pin (on or off).
byte current[inputCount];

// Read an analog value from a multiplexed input on a 
// 74HC5067 multiplexer IC.
int multiplexedAnalogRead(int multiplexedInput)
{
    digitalWrite(multiplexBit0Pin, ((multiplexedInput & 0x1) != 0) ? HIGH : LOW);
    digitalWrite(multiplexBit1Pin, ((multiplexedInput & 0x2) != 0) ? HIGH : LOW);
    digitalWrite(multiplexBit2Pin, ((multiplexedInput & 0x4) != 0) ? HIGH : LOW);
    digitalWrite(multiplexBit3Pin, ((multiplexedInput & 0x8) != 0) ? HIGH : LOW);
    delayMicroseconds(10);  // delay 10 microseconds to allow multiplexer to stabilize.
    return analogRead(analogInPin);
}

// Type of LED modes.
const byte LED_ON = 0; // on solid
const byte LED_OFF = 1; // off solid
const byte LED_SLOW = 2; // slow blink
const byte LED_FAST = 3; // fast blink
const byte LED_NUMBER = 4; // fast blink a number of times, then off for a pause, then repeat.

const long SLOW_MILLIS = 600; // on and off time for slow blink
const long FAST_MILLIS = 300; // on and off time for fast blink and count
const long DELAY_MILLIS = 3500; // delay between counts.

int lastLed = LOW;
long lastLedChangeTime;
byte ledMode;  // LED_SOLID, LED_SLOW, LED_FAST or LED_NUMBER
byte ledNumber; // if LED_NUMBER, number of fast blinks
byte ledCountSoFar; // number of fast blinks so far.

void setLedMode(byte mode, byte number)
{
  ledMode = mode;
  ledNumber = number;
  lastLed = LOW;
  digitalWrite(ledPin, LOW);
  updateLed();
}

void updateLed()
{
  long currentTime = millis();
  
  if (ledMode == LED_ON) {
    digitalWrite(ledPin, HIGH);
    lastLed = HIGH;
  }
  else if (ledMode == LED_OFF) {
    digitalWrite(ledPin, LOW);
    lastLed = LOW;
  }
  else if (ledMode == LED_SLOW || ledMode == LED_FAST) {
    if (currentTime - lastLedChangeTime > (ledMode == LED_SLOW ? SLOW_MILLIS : FAST_MILLIS)) {
      lastLed = (lastLed == HIGH) ? LOW : HIGH;
      digitalWrite(ledPin, lastLed);
      lastLedChangeTime = currentTime;
    }
  }
  else if (ledMode == LED_NUMBER) {
    if (ledCountSoFar == 0) {
      if (currentTime - lastLedChangeTime > DELAY_MILLIS) {
        lastLed = (lastLed == HIGH) ? LOW : HIGH;
        digitalWrite(ledPin, lastLed);
        lastLedChangeTime = currentTime;
        ++ledCountSoFar;
      }
    }
    else {
      if (currentTime - lastLedChangeTime > FAST_MILLIS) {
        lastLed = (lastLed == HIGH) ? LOW : HIGH;
        digitalWrite(ledPin, lastLed);
        lastLedChangeTime = currentTime;
        if (lastLed == LOW)      
          ++ledCountSoFar;
      }
 
      if (ledCountSoFar > ledNumber)
        ledCountSoFar = 0;
    }
  }
}

void setLasers(boolean on)
{
    digitalWrite(laserPin, on ? HIGH : LOW);
}

void gatherCalibrationData()
{
  long start = millis();
  
  for (int i = 0; i < inputCount; ++i) {
    calibrateMin[i] = 1023;
    calibrateMax[i] = 0;
  }
  
  while (millis() - start < 1500) {
    for (int i = 0; i < inputCount; ++i) {
      int value = multiplexedAnalogRead(i);
      if (value < calibrateMin[i])
        calibrateMin[i] = value;
      if (value > calibrateMax[i])
        calibrateMax[i] = value;
        
      delay(1);
    }
    
    updateLed();
  }
}

// Display the range of calibration data that was read.
void displayCalibrationData()
{
  for (int i = 0; i < inputCount; ++i) {
    Serial.print(i+1);
    Serial.print(": ");
    Serial.print(calibrateMin[i]);
    Serial.print(" to ");
    Serial.print(calibrateMax[i]);
    Serial.println();
  }
}

// If the gap in reading between on and off is less than this gap, signal an error.
const int minCalibrationGap = 60;

// Calibrate the photoresistor thresholds. Turn the lasers ON, and read
// the photoresistors, keeping the lowest values detected.
// Turn the lasers OFF, read the photoresistors, keeping the highest values detected.
// Put the threshold halfway between those values. If the gap between on and off is
// less than "minCalibrationGap", indicate an error by blinking the status LED the 
// number of the laser that is in error.
void calibrate()
{
  int minLasersOn[inputCount], maxLasersOff[inputCount];
  
  // Blink LED slowly to indicate calibration going on.
  setLedMode(LED_SLOW, 0);
  
  Serial.println("@"); // Indicate that debugging output follows; do not interpret as data.
  Serial.println("Calibrating light sensors...");
  
  setLasers(true);
  delay(400);  // let lasers get fully powered on.
  gatherCalibrationData();
  Serial.println("Lasers on:");
  displayCalibrationData();
  for (int i = 0; i < inputCount; ++i)
    minLasersOn[i] = calibrateMin[i];
    
  setLasers(false);
  delay(400);  // let lasers get fully powered off.
  gatherCalibrationData();
  Serial.println("Lasers off:");
  displayCalibrationData();
  for (int i = 0; i < inputCount; ++i)
    maxLasersOff[i] = calibrateMax[i];
 
  setLasers(true);
  
  Serial.println("Calibration complete; thresholds are:");
  int error = -1; // Set to first laser we can't calibrate.
  for (int i = 0; i < inputCount; ++i) {
    thresholds[i] = (minLasersOn[i] + maxLasersOff[i]) / 2;
    Serial.print(i+1);
    Serial.print(": ");
    Serial.print(thresholds[i]);
    
    if (minLasersOn[i] - maxLasersOff[i] < minCalibrationGap) {
      thresholds[i] = -1;
      Serial.print("  CALIBRATION FAILED!");
      if (error == -1)
        error = i;
    }
    Serial.println();
  }  
  
  // If we had a calibration error, blink the status LED to show laser with problem.
  if (error >= 0)
    setLedMode(LED_NUMBER, error + 1);
  else
    setLedMode(LED_ON, 0);
  
  Serial.println("~");  // Transition back to output data.
}

void setManualThresholds()
{
  for (int i = 0; i < inputCount; ++i) {
    thresholds[i] = defaultThreshold;
  }
}

void setup() {
  // initialize serial communications at 57600 bps:
  Serial.begin(57600); 
  
  // initialize pins.
  pinMode(multiplexBit0Pin, OUTPUT);
  pinMode(multiplexBit1Pin, OUTPUT);
  pinMode(multiplexBit2Pin, OUTPUT);
  pinMode(multiplexBit3Pin, OUTPUT);
  pinMode(ledPin, OUTPUT);
  pinMode(laserPin, OUTPUT);
  
  setLedMode(LED_ON, 0);
  
  if (autoCalibration)
      calibrate();
  else
      setManualThresholds();
      
  setLasers(true);
}

const long heartbeat = 1000;  // milliseconds between heartbeats.

long lastTimeHeartbeat, currentTime;

void loop() {
    
    updateLed();
    
    currentTime = millis();
    if (currentTime - lastTimeHeartbeat > heartbeat) {
      lastTimeHeartbeat = currentTime;
      Serial.println("~");
    }

    for (int i = 0; i < inputCount; ++i) {
        // Ignore inputs that failed calibration.
        if (thresholds[i] < 0)
            continue;
            
        // Read analog input value.
        int value = multiplexedAnalogRead(i);
        
        // Use threshold to convert into ON or OFF.
        byte newInput = (value >= thresholds[i] ? 1 : 0);
        
        // If different than last value, send notification.
        if (newInput != current[i]) {
            if (newInput)
              Serial.write('a' + i); // HIGH means laser detected -> interruption OFF
            else
              Serial.write('A' + i); // LOW means laser not detected -> interruption ON
         }
        current[i] = newInput;
    }  

    // wait 1 milliseconds before the next loop
    delay(1);                     
}
