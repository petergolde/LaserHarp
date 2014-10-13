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

void setup() {
  // initialize serial communications at 57600 bps:
  Serial.begin(57600); 
  
  // initialize pins.
  pinMode(multiplexBit0Pin, OUTPUT);
  pinMode(multiplexBit1Pin, OUTPUT);
  pinMode(multiplexBit2Pin, OUTPUT);
  pinMode(multiplexBit3Pin, OUTPUT);
}

void loop() {
    for (int i = 0; i < inputCount; ++i) {
        // Read analog input value.
        int value = multiplexedAnalogRead(i);
        
        
        Serial.print(i);
        Serial.print(": ");
        Serial.println(value);
    }  

    // wait 3 seconds before the next loop
    delay(3000);                     
}
