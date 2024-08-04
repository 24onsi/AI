import numpy as np
import random
from matplotlib import pyplot as plt
from tensorflow import keras
from sklearn.preprocessing import StandardScaler

class DataReader():
    def __init__(self):
        self.Xihd_Train, self.Yihd_Train, self.Xihd_Test, self.Yihd_Test = self.IHD_ReadData()
        self.Xstk_Train, self.Ystk_Train, self.Xstk_Test, self.Ystk_Test = self.STK_ReadData()
        self.Xdm_Train, self.Ydm_Train, self.Xdm_Test, self.Ydm_Test = self.DM_ReadData()

    def DM_ReadData(self):
        file = open("data/HTNDM.csv")
        data = []
        file.readline()
        for line in file:
            splt = line.split(",")
            GENDER = int(splt[0])
            AGE = int(splt[1])
            SBP = int(splt[2])
            DBP = int(splt[3])
            GLU = int(splt[4])
            BMI = float(splt[5])

            DIS = int(splt[6])

            if DIS == 1 or DIS == 3:
                DIS = 1
            else:
                DIS = 0

            data.append(((GENDER, AGE, SBP, DBP, GLU, BMI), DIS))

        random.shuffle(data)

        X = []
        Y = []

        for el in data:
            X.append(el[0])
            Y.append(el[1])

        X = np.asarray(X)
        Y = np.asarray(Y)

        scaler = StandardScaler()
        X = scaler.fit_transform(X)

        Xdm_Train = X[:int(len(X)*0.8)]
        Ydm_Train = Y[:int(len(Y)*0.8)]
        Xdm_Test = X[int(len(X)*0.8):]
        Ydm_Test = Y[int(len(Y)*0.8):]

        return Xdm_Train, Ydm_Train, Xdm_Test, Ydm_Test

    def IHD_ReadData(self):
        file = open("data/IHDSTK.csv")
        data = []
        file.readline()
        for line in file:
            splt = line.split(",")
            GENDER = int(splt[0])
            AGE = int(splt[1])
            TC = int(splt[2])
            TG = int(splt[3])
            HDL = int(splt[4])

            IHD = int(splt[7])

            data.append(((GENDER, AGE, TC, TG, HDL), IHD))

        random.shuffle(data)

        X = []
        Y = []

        for el in data:
            X.append(el[0])
            Y.append(el[1])

        X = np.asarray(X)
        Y = np.asarray(Y)

        scaler = StandardScaler()
        X = scaler.fit_transform(X)

        Xihd_Train = X[:int(len(X) * 0.8)]
        Yihd_Train = Y[:int(len(Y) * 0.8)]
        Xihd_Test = X[int(len(X) * 0.8):]
        Yihd_Test = Y[int(len(Y) * 0.8):]

        return Xihd_Train, Yihd_Train, Xihd_Test, Yihd_Test

    def STK_ReadData(self):
        file = open("data/IHDSTK.csv")
        data = []
        file.readline()
        for line in file:
            splt = line.split(",")
            GENDER = int(splt[0])
            AGE = int(splt[1])
            TC = int(splt[2])
            TG = int(splt[3])
            HDL = int(splt[4])

            STK = int(splt[8])

            data.append(((GENDER, AGE, TC, TG, HDL), STK))

        random.shuffle(data)

        X = []
        Y = []

        for el in data:
            X.append(el[0])
            Y.append(el[1])

        X = np.asarray(X)
        Y = np.asarray(Y)

        scaler = StandardScaler()
        X = scaler.fit_transform(X)

        Xstk_Train = X[:int(len(X) * 0.8)]
        Ystk_Train = Y[:int(len(Y) * 0.8)]
        Xstk_Test = X[int(len(X) * 0.8):]
        Ystk_Test = Y[int(len(Y) * 0.8):]

        return Xstk_Train, Ystk_Train, Xstk_Test, Ystk_Test

EPOCHS = 20
dr = DataReader()

model = keras.Sequential([
    keras.layers.Dense(128, input_dim=5, activation="relu"),
    keras.layers.BatchNormalization(),
    keras.layers.Dropout(rate=0.5),
    keras.layers.Dense(64, activation="relu"),
    keras.layers.BatchNormalization(),
    keras.layers.Dropout(rate=0.5),
    keras.layers.Dense(32, activation="relu"),
    keras.layers.Dense(1, activation='sigmoid')
])

optimizer = keras.optimizers.Adam(learning_rate=0.001)
model.compile(optimizer=optimizer, metrics=["accuracy"],
              loss="binary_crossentropy")

print(model.summary())

early_stop = keras.callbacks.EarlyStopping(monitor='val_loss', patience=5, restore_best_weights=True)
model_checkpoint = keras.callbacks.ModelCheckpoint('best_model.h5', monitor='val_loss', save_best_only=True)

history = model.fit(dr.Xstk_Train, dr.Ystk_Train, epochs=EPOCHS, batch_size=64,
                    validation_data=(dr.Xstk_Test, dr.Ystk_Test),
                    callbacks=[early_stop, model_checkpoint])

# 모델 전체 저장
model.save('test.h5')
