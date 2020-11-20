#include <inttypes.h>

typedef intptr_t cell_t;

#define NEXT \
  w = (cell_t *) *ip++; \
  x = (cell_t *) *w; \
  goto *x; 
  
void run(cell_t *memory, cell_t *dstack, cell_t *rstack) {
  register cell_t *ip asm("rdi") = memory;
  register cell_t *w asm("rcx");
  register cell_t *x;
  register cell_t *sp asm("rbp") = dstack;
  register cell_t tos asm("rbx");
  register cell_t *rp asm("r8") = rstack;

  tos = *sp--;

  memory[0] = (cell_t) && dup;
  memory[1] = (cell_t) && and;
  memory[2] = (cell_t) && or;
  memory[3] = (cell_t) && add;
  memory[4] = (cell_t) && load;
  memory[5] = (cell_t) && store;
  memory[6] = (cell_t) && dolit;
  memory[7] = (cell_t) && docolon;
  memory[8] = (cell_t) && doexit;

dup:
  *++sp = tos;
  NEXT;

add:
  tos += *sp--;
  NEXT;

and:
  tos &= *sp--;
  NEXT;

or:
  tos |= *sp--;
  NEXT;

dolit:
  *++sp = tos;
  tos = *ip++;
  NEXT;

docolon:
  *++rp = (cell_t) ip;
  ip = ((cell_t *) w) + 1;
  NEXT;

doexit:
  ip = (cell_t *) *rp--;
  NEXT;

load:
  tos = * (cell_t *) tos;
  NEXT;

store:
  *(cell_t *) tos = *sp--;
  tos = *sp--;
  NEXT;
}
