# python 版 weaving_client

#### 文件介绍：

**weaving_client.py:**  python 版的 weaving_client 框架，使用 python3

**weaving_app.py:**  作为示例程序，调用了 weaving_client.py



#### 使用步骤：

##### 1.用一个IP地址加端口（如 192.168.0.20:5678）初始化一个 WeavingClinet 的对象：

```python
server_addr = '192.168.0.20:5678'
wc = WeavingClient(server_addr)
```



##### 2.使用 wc.reg_wcmd 为你感兴趣的命令字注册无返回值的回调函数（协议解析出的有效数据字节串将作为参数传递给它）：

```python
WCMD_HEARTBEAT = 0x99 # WCMD 指 weaving 框架里的命令字 "weaving cmd"
WCMD_SOMECMD

def some_function(bdata:bytes):
    print('解析出的有效数据字节序列：{}'.format(bdata))

wc.reg_wcmd(WCMD_HEARTBEAT,None) # 不处理心跳包，其实不使用 reg_wcmd 注册的命令字默认不处理
wc.reg_wcmd(WCMD_SOMECMD,some_function)

```



##### 3.指定连接到服务端时的回调函数，通常需要用这个来注册客户端

```python
reg_msg = {'cmd':'reg','id':'12345678'} # 注意这个 cmd 相对于 wcmd 只是一个普通数据的一部分

def send_data(bdata:bytes):
    wc.send_data(CMD_SOMECMD,bdata)

def send_msg(msg:dict):
    bdata = json.dumps(msg).encode()
    send_data(bdata)

def reg_self():
    self.send_msg(reg_msg)

wc.on_connect = reg_self
```



##### 4.现在完成了基本配置，可以启动了：

```python
def start():
    wc.start()
    print('Now we can do other things...')
    print('This is just one example, if you want exit, you can:')
    print('on windows: print Ctrl + Pause Break')
    print('on linux: print Ctrl + C')

start()
```



##### 5.更详细的使用方法参见 weaving_app.py 或自由定制

