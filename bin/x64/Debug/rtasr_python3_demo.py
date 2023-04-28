# -*- encoding:utf-8 -*-
import hashlib
import hmac
import base64
from socket import *
import json, time, threading
from websocket import create_connection
import websocket
from urllib.parse import quote
import logging

totalstr=""
fulldata=""
arrdata=[]
# reload(sys)
# sys.setdefaultencoding("utf8")
class Client():
    def __init__(self):
        base_url = "ws://rtasr.xfyun.cn/v1/ws"
        ts = str(int(time.time()))
        tt = (app_id + ts).encode('utf-8')
        md5 = hashlib.md5()
        md5.update(tt)
        baseString = md5.hexdigest()
        baseString = bytes(baseString, encoding='utf-8')

        apiKey = api_key.encode('utf-8')
        signa = hmac.new(apiKey, baseString, hashlib.sha1).digest()
        signa = base64.b64encode(signa)
        signa = str(signa, 'utf-8')
        self.end_tag = "{\"end\": true}"

        self.ws = create_connection(base_url + "?appid=" + app_id + "&ts=" + ts + "&signa=" + quote(signa))
        self.trecv = threading.Thread(target=self.recv)
        self.trecv.start()

    def send(self, file_path):
        file_object = open(file_path, 'rb')
        try:
            index = 1
            while True:
                chunk = file_object.read(1280)
                if not chunk:
                    break
                self.ws.send(chunk)

                index += 1
                time.sleep(0.04)
        finally:
            file_object.close()

        self.ws.send(bytes(self.end_tag.encode('utf-8')))
        # print("send end tag success")

    def recv(self):
        global totalstr
        try:
            while self.ws.connected:
                result = str(self.ws.recv())
                if len(result) == 0:
                    # print("receive result end")
                    break
                result_dict = json.loads(result)
                # 解析结果
                if result_dict["action"] == "started":
                    i=1
                    # print("handshake success, result: " + result)

                if result_dict["action"] == "result":
                    result_1 = result_dict
                    # result_2 = json.loads(result_1["cn"])
                    # result_3 = json.loads(result_2["st"])
                    # result_4 = json.loads(result_3["rt"])
                    tmpdata=result_1["data"]
                    # print("rtasr data: %s" % (tmpdata))
                    tmpdata = json.loads(tmpdata)
                    type = tmpdata["cn"]["st"]["type"]
                    # print("rtasr type=%s" % (tmpdata["cn"]["st"]["type"]))
                    tmpdata=tmpdata["cn"]["st"]["rt"]
                    # tmpdata1 = json.loads(tmpdata1)
                    totalstr = ""
                    for i in range(len(tmpdata)):
                        # print("rtasr tmpdata tmpdata=%s" % tmpdata[i])
                        wsdata = tmpdata[i]["ws"]
                        for j in range(len(wsdata)):
                            # print("rtasr tmpdata wsdata=%s" % wsdata[j])
                            cwdata = wsdata[j]["cw"]
                            for k in range(len(cwdata)):
                                totalstr += cwdata[k]['w']
                                # print("rtasr tmpdata cwdata=%s w=%s" % (cwdata[k], cwdata[k]['w']))
                    if type == "1":
                        fulldata = totalstr
                        # print("totalstr1111=%s " % (totalstr))
                    else:
                        arrdata.append(totalstr)
                        totalstr = ""
                    tmpdata=""
                    for i in range(len(arrdata)):
                        tmpdata += arrdata[i]
                        # tmpdata += ","
                    tmpdata=tmpdata+totalstr
                    # with open('read.txt',mode='w') as f:
                    #     f.write(tmpdata)
                    print("%s" % tmpdata)
                        # fulldata += ","
                        # fulldata += totalstr
                    
                    
                    # tmpdata = json.loads(tmpdata1)
                    # print("rtasr data: %s" % tmpdata)
                    # tmpdata=tmpdata["cn"]
                    # tmpdata = json.loads(tmpdata)

                    # tmpdata=tmpdata["st"]
                    # tmpdata = json.loads(tmpdata)

                    # tmpdata=tmpdata["rt"]
                    # tmpdata = json.loads(tmpdata)

                    # tmpdata=tmpdata["ws"]
                    # tmpdata = json.loads(tmpdata)

                    # tmpdata=tmpdata["cw"]
                    # tmpdata = json.loads(tmpdata)

                    # tmpdata=tmpdata["ws"]
                    # tmpdata = json.loads(tmpdata)
                    # tmpdata = json.loads(tmpdata["tmpdata"])

                    # print("rtasr result11: %d" % tmpdata["seg_id"])
                    # print("rtasr result: " + result_dict)
                    # print("data=%d: " % result_1["data"]["seg_id"])

                if result_dict["action"] == "error":
                    print("rtasr error: " + result)
                    self.ws.close()
                    return
        except websocket.WebSocketConnectionClosedException:
            print("receive result end")

    def close(self):
        self.ws.close()
        print("connection closed")


if __name__ == '__main__':
    logging.basicConfig()

    app_id = "33e889c1"
    api_key = "1350eebbdc2a4f40816f83b005edd93f"
    file_path = r"./chineseFile.wav"

    client = Client()
    client.send(file_path)
    # print("totalstr=%s" % totalstr)
    tmpdata=""
    for i in range(len(arrdata)):
        tmpdata += arrdata[i]
        # tmpdata += ","
    # print("%s" % tmpdata)