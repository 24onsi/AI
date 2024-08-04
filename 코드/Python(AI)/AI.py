import socket
import threading
import numpy as np
import json
from tensorflow import keras

# 모델 불러오기
def read_model():
    print('모델 로드')
    ihd_model = keras.models.load_model('model/IHD.h5')
    stk_model = keras.models.load_model('model/STK.h5')
    dm_model = keras.models.load_model('model/DM.h5')

    return ihd_model, stk_model, dm_model

def read_json(jsonmsg):
    print('json 읽기')
    gender = jsonmsg['Gender']
    age = jsonmsg['Age']
    smoke = jsonmsg['Smoke']
    drink = jsonmsg['Drink']
    sbp = jsonmsg['SBP']
    dbp = jsonmsg['DBP']
    tc = jsonmsg['TC']
    tg = jsonmsg['TG']
    hdl = jsonmsg['HDL']
    glu = jsonmsg['GLU']
    bmi = jsonmsg['BMI']

    print('확인하기')
    print(gender, age, smoke, drink, sbp, dbp, tc, tg, hdl, glu, bmi)

    dis_data = np.array([[gender, age, sbp, dbp, glu, bmi]])
    cvd_data = np.array([[gender, age, tc, tg, hdl]])

    return dis_data, cvd_data , sbp, dbp

def reference_value(sbp, dbp):

    if sbp < 120 and dbp < 80:
        htn = 0
    elif sbp < 130:
        htn = 15
    elif sbp < 140 and dbp < 90:
        htn = 30
    elif sbp < 150:
        htn = 50
    elif sbp < 160 and dbp < 100:
        htn = 75
    else:
        htn = 100

    print(f'혈압 위험도 : {htn}')

    return htn

def predict_value(dis_data, cvd_data):

    ihd_model, stk_model, dm_model = read_model()

    dm_predict = dm_model.predict(dis_data)
    ihd_predict = ihd_model.predict(cvd_data)
    stk_predict = stk_model.predict(cvd_data)

    dm_predict = int(dm_predict[0][0] * 100)
    ihd_predict = int(ihd_predict[0][0] * 100)
    stk_predict = int(stk_predict[0][0] * 100)

    print(f'심장 위험도 : {ihd_predict}')
    print(f'뇌 위험도 : {stk_predict}')
    print(f'당뇨 위험도 : {dm_predict}')

    return ihd_predict, stk_predict, dm_predict

def handler(client_socket, addr):
    print('Connected by', addr)
    try:
        data = client_socket.recv(1024)

        if not data:
            print("클라이언트 연결 끊김: ", addr)
        else:
            jsonmsg = json.loads(data.decode('utf-8'))
            print('제이슨 메세지 : ', jsonmsg)

            dis, cvd, sbp, dbp = read_json(jsonmsg)
            ihd, stk, dm = predict_value(dis, cvd)
            htn = reference_value(sbp, dbp)

            sendmsg = {"IHD":ihd,
                        "STK": stk,
                        "HTN": htn,
                        "DM": dm}

            jsonsend = json.dumps(sendmsg)  # Python 객체를 JSON 문자열로 변환
            print("보낸 메세지 :", jsonsend)
            client_socket.sendall(jsonsend.encode())

    except json.JSONDecodeError:
        print("JSON 디코딩 오류 발생")

    except ConnectionResetError:
        print("클라이언트 연결 끊김: ", addr)

    except Exception as e:
        print("예기치 않은 오류 발생: ", e)
    finally:
        client_socket.close()
        print("클라가 연결 끊음")

server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
server_socket.bind(('', 5000))
server_socket.listen()

print("서버 열림!")

try:
    while True:
        client_socket, addr = server_socket.accept()
        thr = threading.Thread(target=handler, args=(client_socket, addr))
        thr.start()

except Exception as e:
    print("Server exception:", e)
finally:
    server_socket.close()