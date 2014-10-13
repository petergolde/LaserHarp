
void setup()
{
  Serial.begin(9600); 
  // initialize digital pin 13 as an output.
  pinMode(13, OUTPUT);
}

void loop()
{ 
  for (int i = 0; i < 8; i ++)
  {
    Serial.write('0' + i);    
    digitalWrite(13, HIGH);   // turn the LED on (HIGH is the voltage level)
    delay(250);              // wait for a second
    digitalWrite(13, LOW);    // turn the LED off by making the voltage LOW
    delay(250);     
  }
} 
