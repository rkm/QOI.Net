CXX = g++
TARGETS = encode decode

.PHONY: all
all: $(TARGETS)

.PHONE: clean
clean:
	rm $(TARGETS)

$(TARGETS): %: %.cpp
	$(CXX) $^ -o $@
