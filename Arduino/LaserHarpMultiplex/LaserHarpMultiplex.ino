/*
  Analog input, serial output
 */

// These constants won't change.  They're used to give names
// to the pins used:
const int analogInPin = A0;  // Analog input pin that the potentiometer is attached to

const int multiplexBit0Pin = 2;  // Pin that controls bit 0 of the multiplexer
const int multiplexBit1Pin = 3;  // Pin that controls bit 0 of the multiplexer
const int multiplexBit2Pin = 4;  // Pin that controls bit 0 of the multiplexer
const int multiplexBit3Pin = 5;  // Pin that controls bit 0 of the multiplexer

const int inputCount = 8;  // Number of inputs we are reading

// Thresholds for input on each pin. >= this value is ON, < this value is OFF.
int thresholds[inputCount] = { 400, 400, 400, 400, 400, 400, 400, 400 };

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

// Pin for status LED.
const int ledPin = 9;

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

void setup() {
  // initialize serial communications at 57600 bps:
  Serial.begin(57600); 
  
  // initialize pins.
  pinMode(multiplexBit0Pin, OUTPUT);
  pinMode(multiplexBit1Pin, OUTPUT);
  pinMode(multiplexBit2Pin, OUTPUT);
  pinMode(multiplexBit3Pin, OUTPUT);
  pinMode(ledPin, OUTPUT);
  
  setLedMode(LED_ON, 0);
}

void loop() {
    updateLed();

    for (int i = 0; i < inputCount; ++i) {
      
        // Read analog input value.
        int value = multiplexedAnalogRead(i);
        
        // Use threshold to convert into ON or OFF.
        byte newInput = (value >= thresholds[i] ? 1 : 0);
        
        // If different than last value, send notification.
        if (newInput != current[i]) {
            //Serial.print(i);
            //Serial.print(newInput ? '+' : '-');
            //Serial.println();
            if (newInput){
              Serial.print(i);
            }
        }
        current[i] = newInput;
    }  

    // wait 2 milliseconds before the next loop
    delay(2);                     
}
