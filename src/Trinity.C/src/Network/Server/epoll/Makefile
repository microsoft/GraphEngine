CC=g++
CFLAGS=-I.
all: TrinitySocketServer.o TrinityServer.o
	$(CC) -o socketserver.tapp TrinitySocketServer.o TrinityServer.o -I. -I../../../../include -I../../../../src -std=c++11 -pthread

TrinityServer.o: 
	$(CC) -o TrinityServer.o -c ../posix/TrinityServer.cpp -I.. -I../../../../include -I../../../../src -std=c++11 -pthread

TrinitySocketServer.o: TrinitySocketServer.cpp
	$(CC) -o TrinitySocketServer.o -c TrinitySocketServer.cpp -I. -I../../../../include -I../../../../src -std=c++11 -pthread

run: socketserver.tapp
	./socketserver.tapp
clean:
	rm -f *.o *.tapp
