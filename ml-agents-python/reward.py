import os
import win32pipe, win32file

pipe_name = r'\\.\pipe\UnityToPythonPipe'

print("파이프 서버 대기 중...")

pipe = win32pipe.CreateNamedPipe(
    pipe_name,
    win32pipe.PIPE_ACCESS_DUPLEX,
    win32pipe.PIPE_TYPE_MESSAGE | win32pipe.PIPE_READMODE_MESSAGE | win32pipe.PIPE_WAIT,
    1,
    65536,
    65536,
    0,
    None
)   

win32pipe.ConnectNamedPipe(pipe, None)
print("파이프 연결됨")