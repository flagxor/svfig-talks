all: dump threading.o threading2.o

CFLAGS=-O2

dump: threading2.o
	objdump -d $<

threading.o: threading.c

threading2.o: threading2.c

clean:
	rm -f *.o
