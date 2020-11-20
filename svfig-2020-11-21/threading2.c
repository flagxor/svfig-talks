#include <inttypes.h>

typedef intptr_t cell_t;

#define IDT_NEXT \
  w = (cell_t *) *ip++; \
  x = (cell_t *) *w; \
  goto *x;

#define DT_NEXT \
  w = (cell_t *) *ip++; \
  goto *w;

#define CORE_WORDS \
  X(dup, *++sp = tos) \
  X(add, tos += *sp--) \
  X(and, tos &= *sp--) \
  X(or, tos |= *sp--) \
  X(dolit, *++sp = tos; tos = *ip++) \
  X(docolon, *++rp = (cell_t) ip; ip = ((cell_t *) w) + 1) \
  X(doexit, ip = (cell_t *) *rp--) \
  X(load, tos = * (cell_t *) tos) \
  X(store, *(cell_t *) tos = *sp--; tos = *sp--) \


#define COMMON_REGS \
  register cell_t *ip asm("rdi") = memory; \
  register cell_t *sp asm("rsi") = dstack; \
  register cell_t *rp asm("r9") = rstack; \
  register cell_t tos asm("rbx") = *sp--; \
  register cell_t *w asm("rcx"); \
  int i = 0; \


void indirect_threaded(cell_t *memory, cell_t *dstack, cell_t *rstack) {
  COMMON_REGS;
  register cell_t *x;

#define X(name, code) memory[i++] = (cell_t) && name;
CORE_WORDS
#undef X

#define X(name, code) name: code; IDT_NEXT;
CORE_WORDS
#undef X
}

void direct_threaded(cell_t *memory, cell_t *dstack, cell_t *rstack) {
  COMMON_REGS;

#define X(name, code) memory[i++] = (cell_t) && name;
CORE_WORDS
#undef X

#define X(name, code) name: code; DT_NEXT;
CORE_WORDS
#undef X
}
