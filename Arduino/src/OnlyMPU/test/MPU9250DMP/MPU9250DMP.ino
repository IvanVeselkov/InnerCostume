#include "I2Cdev.h"

#include "MPU9250_9Axis_MotionApps41.h"


// is used in I2Cdev.h
#if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
    #include "Wire.h"
#endif

MPU9250 mpu;



#define OUTPUT_READABLE_QUATERNION


#define INTERRUPT_PIN 2  // используйте контакт 2 на Arduino Uno и большинстве плат
#define LED_PIN 13 
bool blinkState = false;

// Переменные управления / состояния MPU
bool dmpReady = false;  // установить true, если инициализация DMP прошла успешно
uint8_t mpuIntStatus;   // хранит текущий байт состояния прерывания от MPU
uint8_t devStatus;      // возвращать статус после каждой операции с устройством (0 = успех,! 0 = ошибка)
uint16_t packetSize;    // ожидаемый размер пакета DMP (по умолчанию 42 байта)
uint16_t fifoCount;     // подсчет всех байтов в текущий момент в FIFO
uint8_t fifoBuffer[64]; // Буфер хранения FIFO
float f_mag[3];         // данные магнитометра после преобразования согласно даташиту
float d_mag[4];

// ориентация / движение

Quaternion q;           // [w, x, y, z]         quaternion container
// VectorFloat v_mag;      // [x, y, z]            вектор магнитного поля
int16_t mag[3];         // сырые данные магнитометра
Quaternion q_mag;       // вспомогательный кватернион магнитометра
VectorInt16 raw_accel;
VectorInt16 accel;
VectorFloat gravity;
VectorInt16 gyro;
float* gyrof= new float[3];
bool first_data;
VectorInt16 first_accel;

volatile bool mpuInterrupt = false;    
void dmpDataReady() {
    mpuInterrupt = true;
}



//функция не дает выходить за пределы
float clamp(float v, float minv, float maxv){
    if( v>maxv )
        return maxv;
    else if( v<minv )
        return minv;
    return v;
}

//функция высчитывания вектора гравитации
uint16_t dmpGetGravity(VectorFloat *v, Quaternion *q) {
    v -> x = 2 * (q -> x*q -> z - q -> w*q -> y);
    v -> y = 2 * (q -> w*q -> x + q -> y*q -> z);
    v -> z = q -> w*q -> w - q -> x*q -> x - q -> y*q -> y + q -> z*q -> z;
    return 0;
}

//функция вычисления линейного ускорения
uint8_t dmpGetLinearAccel(VectorInt16 *v, VectorInt16 *vRaw, VectorFloat *gravity) {
    // get rid of the gravity component (+1g = +4096 in standard DMP FIFO packet)
    v -> x = vRaw -> x - gravity -> x*4096*2;
    v -> y = vRaw -> y - gravity -> y*4096*2;
    v -> z = vRaw -> z - gravity -> z*4096*2;
    return 0;
}

void setup() {
    #if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
        Wire.begin();
        Wire.setClock(400000);
    #elif I2CDEV_IMPLEMENTATION == I2CDEV_BUILTIN_FASTWIRE
        Fastwire::setup(400, true);
    #endif
    Serial.begin(115200);
    while (!Serial);
    mpu.initialize();
    pinMode(INTERRUPT_PIN, INPUT);
    Serial.println(F("Testing device connections..."));
    Serial.println(mpu.testConnection() ? F("MPU9250 connection successful") : F("MPU9250 connection failed"));
    devStatus = mpu.dmpInitialize();
    if (devStatus == 0) {
        mpu.setDMPEnabled(true);
        attachInterrupt(digitalPinToInterrupt(INTERRUPT_PIN), dmpDataReady, RISING);
        mpuIntStatus = mpu.getIntStatus();
        dmpReady = true;
        packetSize = mpu.dmpGetFIFOPacketSize();
    } else {
    }
    pinMode(LED_PIN, OUTPUT);
    first_data=false;
}
void loop() {
    if (!dmpReady) 
    {
      Serial.println("fail");
      return;
    }
    mpuInterrupt = false;
    mpuIntStatus = mpu.getIntStatus();
    fifoCount = mpu.getFIFOCount();   
    if ((mpuIntStatus & 0x10) || fifoCount == 1024) {        
        mpu.resetFIFO();
        Serial.println(F("FIFO overflow!"));   
    } else if (mpuIntStatus & 0x02) {
        while (fifoCount < packetSize) fifoCount = mpu.getFIFOCount();
        mpu.getFIFOBytes(fifoBuffer, packetSize);
        fifoCount -= packetSize;
        #ifdef OUTPUT_READABLE_QUATERNION
            mpu.dmpGetQuaternion(&q, fifoBuffer);
            Serial.print(q.x);
            Serial.print("\t");
            Serial.print(q.y);
            Serial.print("\t");
            Serial.print(q.z);
            Serial.print("\t");
            Serial.println(q.w);      
        #endif
        blinkState = !blinkState;
        digitalWrite(LED_PIN, blinkState);
    }
}
