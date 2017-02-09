CC=g++
CFLAGS=-I.

clientsocket.tapp: ClientSocket.o TrinityClientSocketTest.o
	$(CC) -o clientsocket.tapp ClientSocket.o TrinityClientSocketTest.o -std=c++11 -pthread -DSOCKET_CLIENT_TEST

TrinityClientSocketTest.o: TrinityClientSocketTest.cpp
	$(CC) -c -o TrinityClientSocketTest.o TrinityClientSocketTest.cpp -I. -I../../../include -I../../../src -std=c++11 -pthread -DSOCKET_CLIENT_TEST

ClientSocket.o: 
	$(CC) -c -o ClientSocket.o posix/ClientSocket.cpp -I. -I../../../include -I../../../src -std=c++11 -pthread -DSOCKET_CLIENT_TEST

run: clientsocket.tapp
	./clientsocket.tapp

clean:
	rm -f *.o *.tapp
