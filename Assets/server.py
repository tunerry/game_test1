import socket
import pymysql
import json


def login(cursor, username, passwd):
    sql = "select name, passwd from user where name='{}'".format(username)
    cursor.execute(sql)
    res = cursor.fetchall()
    error = '0'
    if not res:
        # 建立新账户
        print("Create New Account.")
        sql = "INSERT INTO user (name, passwd) VALUES ('{}', '{}')".format(username, passwd)
        cursor.execute(sql)
    elif passwd != res[0]['passwd']:
        print("password error!")
        error = '1'
    return error


def rank(cursor):
    sql = "select * from ranklist"
    cursor.execute(sql)
    res = cursor.fetchall()
    resjson = json.dumps(res)

    return resjson


def update(cursor, rankjson):
    for r in rankjson:
        sql = "UPDATE ranklist SET name = '{}', point = '{}' WHERE rank='{}'".format(r['name'], r['point'], r['rank'])
        cursor.execute(sql)
    return '0'


if __name__ == "__main__":
    serverSocket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    connection = pymysql.connect(host='127.0.0.1', port=3306, user='root', password='123456', db='test',
                                 cursorclass=pymysql.cursors.DictCursor)
    cursor = connection.cursor()
    host = '127.0.0.1'
    port = 10086

    serverSocket.bind((host, port))
    serverSocket.listen(5)

    print("正在监听...")
    while True:
        clientSocket, clientAddr = serverSocket.accept()
        print("连接建立：{}".format(clientAddr))
        recvData = clientSocket.recv(1024)
        recvMsg = recvData.decode("utf-8")
        recvJson = json.loads(recvMsg)
        print("收到数据：" + recvMsg)
        method = recvJson['method']
        if method == 'login':
            username = recvJson['name']
            passwd = recvJson['passwd']
            reply = login(cursor, username, passwd)
        elif method == 'rank':
            reply = rank(cursor)
        elif method == 'update':
            data = recvJson['data']
            reply = update(cursor, data)
        connection.commit()
        clientSocket.send(reply.encode("utf-8"))
        print("返回数据：{}".format(reply))