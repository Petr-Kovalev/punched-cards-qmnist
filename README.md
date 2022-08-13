### 'Punched cards' for QMNIST

*Object recognition by sparse random binary data lookup. Based on [this article](https://petr-kovalev.medium.com/punched-cards-object-recognition-97523a98857b)*

Performing single-shot QMNIST handwritten digits recognition by lookup over the most representative sparse input bit sets of the training data (out of 28⋅28⋅8 = 6272 bits per training sample)

Bit vector set similarity evaluation using the maximum spanning tree is described in [this article](https://petr-kovalev.medium.com/bit-vector-set-similarity-maximum-spanning-tree-2121b05c7b29)

The same algorithm applied to the Fashion-MNIST dataset [is here](https://github.com/Petr-Kovalev/punched-cards-fashion-mnist)

The same algorithm applied to the Oracle-MNIST dataset [is here](https://github.com/Petr-Kovalev/punched-cards-oracle-mnist)

### Program output:
```
Punched card bit length: 8

Top punched card per input:
Training results: 25213 correct recognitions of 60000
Test results: 25307 correct recognitions of 60000

Top 39 (5%) punched cards per input:
Training results: 42675 correct recognitions of 60000
Test results: 42778 correct recognitions of 60000

All punched cards:
Training results: 46904 correct recognitions of 60000
Test results: 47059 correct recognitions of 60000

Punched card bit length: 16

Top punched card per input:
Training results: 25598 correct recognitions of 60000
Test results: 25597 correct recognitions of 60000

Top 19 (5%) punched cards per input:
Training results: 42764 correct recognitions of 60000
Test results: 42701 correct recognitions of 60000

All punched cards:
Training results: 46953 correct recognitions of 60000
Test results: 47024 correct recognitions of 60000

Punched card bit length: 32

Top punched card per input:
Training results: 31531 correct recognitions of 60000
Test results: 31876 correct recognitions of 60000

Top 9 (5%) punched cards per input:
Training results: 43604 correct recognitions of 60000
Test results: 43514 correct recognitions of 60000

All punched cards:
Training results: 47098 correct recognitions of 60000
Test results: 47147 correct recognitions of 60000

Punched card bit length: 64

Top punched card per input:
Training results: 35021 correct recognitions of 60000
Test results: 35505 correct recognitions of 60000

Top 4 (5%) punched cards per input:
Training results: 42779 correct recognitions of 60000
Test results: 42978 correct recognitions of 60000

All punched cards:
Training results: 47002 correct recognitions of 60000
Test results: 47103 correct recognitions of 60000

Punched card bit length: 128

Top punched card per input:
Training results: 39537 correct recognitions of 60000
Test results: 39672 correct recognitions of 60000

Top 2 (5%) punched cards per input:
Training results: 43182 correct recognitions of 60000
Test results: 43256 correct recognitions of 60000

All punched cards:
Training results: 46998 correct recognitions of 60000
Test results: 47049 correct recognitions of 60000

Punched card bit length: 256

Top punched card per input:
Training results: 43019 correct recognitions of 60000
Test results: 43019 correct recognitions of 60000

Top 1 (5%) punched cards per input:
Training results: 43019 correct recognitions of 60000
Test results: 43019 correct recognitions of 60000

All punched cards:
Training results: 46885 correct recognitions of 60000
Test results: 46980 correct recognitions of 60000

Press "Enter" to exit the program...
```
