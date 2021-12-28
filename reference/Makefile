CXX = g++
TARGETS = encode decode

.PHONY: all
all: $(TARGETS)

.PHONY: clean
clean:
	rm $(TARGETS)

$(TARGETS): %: %.cpp
	$(CXX) $^ -o $@
