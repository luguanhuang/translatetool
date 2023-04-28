import sys
import urllib.request
import json
import os

def getdata():
    APIkey = "AQVNzYDqawlK2hMFQPIi9nV5ml9ofa1rJF5-lMJE"  

    with open("tmpFile.wav", "rb") as file:
        #data = file.read(1024*1024)
        data = file.read()

    #print("len=%d" % len(data))
    file.close()
    os.remove("tmpFile.wav")
    params = "&".join([
        "topic=general",
        "lang=ru-RU",
        "format=lpcm",
        "sampleRateHertz=16000"
    ])

    url = urllib.request.Request("https://stt.api.cloud.yandex.net/speech/v1/stt:recognize?%s" % params, data=data)

    url.add_header("Authorization", "Api-Key %s" % APIkey)

    responseData = urllib.request.urlopen(url).read().decode('UTF-8')
    decodedData = json.loads(responseData)

    if decodedData.get("error_code") is None:
        # print(decodedData.get("result"))
        return decodedData.get("result")


if __name__ == '__main__':
    print(getdata())

# print(sys.path)
# sys.path.append('python的site-packages路径')

# print(sys.modules["urllib.request"])

# print(sys.modules["urllib.parse"])