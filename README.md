### 'Punched cards' for QMNIST

*Object recognition by sparse random binary data lookup. Based on [this article](https://petr-kovalev.medium.com/punched-cards-object-recognition-97523a98857b)*

Performing single-shot QMNIST handwritten digits recognition by lookup over the most representative sparse input bit sets of the training data (out of 28⋅28⋅8 = 6272 bits per training sample)

Bit vector set similarity evaluation using the maximum spanning tree is described in [this article](https://petr-kovalev.medium.com/bit-vector-set-similarity-maximum-spanning-tree-2121b05c7b29)

The same algorithm applied to the Fashion-MNIST dataset [is here](https://github.com/Petr-Kovalev/punched-cards-fashion-mnist)

The same algorithm applied to the Oracle-MNIST dataset [is here](https://github.com/Petr-Kovalev/punched-cards-oracle-mnist)

### Program output:
```
Punched card bit length: 8

Average single-shot correct recognitions on fine-tune iteration: 11541, 11181

Top punched card per input:
Training results: 21258 correct recognitions of 60000
Test results: 21444 correct recognitions of 60000

Top 39 (5%) punched cards per input:
Training results: 41825 correct recognitions of 60000
Test results: 41794 correct recognitions of 60000

All punched cards:
Training results: 47035 correct recognitions of 60000
Test results: 46999 correct recognitions of 60000

Punched card bit length: 16

Average single-shot correct recognitions on fine-tune iteration: 15894, 15864

Top punched card per input:
Training results: 25346 correct recognitions of 60000
Test results: 25817 correct recognitions of 60000

Top 19 (5%) punched cards per input:
Training results: 43013 correct recognitions of 60000
Test results: 43118 correct recognitions of 60000

All punched cards:
Training results: 48409 correct recognitions of 60000
Test results: 48386 correct recognitions of 60000

Punched card bit length: 32

Average single-shot correct recognitions on fine-tune iteration: 21380, 22069, 22407, 22614, 22758, 22857, 22929, 22986, 23029, 23063, 23088, 23110, 23127, 23138, 23147, 23154, 23158, 23162, 23163, 23163, 23167, 23165

Top punched card per input:
Training results: 25753 correct recognitions of 60000
Test results: 26146 correct recognitions of 60000

Top 9 (5%) punched cards per input:
Training results: 45274 correct recognitions of 60000
Test results: 45153 correct recognitions of 60000

All punched cards:
Training results: 50568 correct recognitions of 60000
Test results: 50548 correct recognitions of 60000

Punched card bit length: 64

Average single-shot correct recognitions on fine-tune iteration: 27644, 28762, 29381, 29780, 30063, 30271, 30436, 30567, 30677, 30770, 30850, 30913, 30973, 31022, 31067, 31107, 31139, 31169, 31197, 31220, 31243, 31262, 31279, 31296, 31311, 31322, 31336, 31345, 31355, 31365, 31375, 31380, 31388, 31394, 31400, 31407, 31410, 31416, 31420, 31425, 31429, 31432, 31437, 31437

Top punched card per input:
Training results: 31184 correct recognitions of 60000
Test results: 31574 correct recognitions of 60000

Top 4 (5%) punched cards per input:
Training results: 44800 correct recognitions of 60000
Test results: 44836 correct recognitions of 60000

All punched cards:
Training results: 51391 correct recognitions of 60000
Test results: 51308 correct recognitions of 60000

Punched card bit length: 128

Average single-shot correct recognitions on fine-tune iteration: 33881, 34919, 35548, 35988, 36313, 36562, 36762, 36924, 37060, 37178, 37277, 37363, 37440, 37509, 37570, 37625, 37674, 37718, 37759, 37796, 37830, 37861, 37891, 37912, 37938, 37958, 37980, 37998, 38016, 38033, 38050, 38064, 38078, 38091, 38104, 38113, 38126, 38137, 38147, 38157, 38165, 38174, 38182, 38190, 38197, 38207, 38211, 38216, 38221, 38227, 38234, 38234

Top punched card per input:
Training results: 40044 correct recognitions of 60000
Test results: 40470 correct recognitions of 60000

Top 2 (5%) punched cards per input:
Training results: 45813 correct recognitions of 60000
Test results: 45867 correct recognitions of 60000

All punched cards:
Training results: 52091 correct recognitions of 60000
Test results: 51986 correct recognitions of 60000

Punched card bit length: 256

Average single-shot correct recognitions on fine-tune iteration: 39489, 40450, 41040, 41449, 41749, 41993, 42181, 42327, 42465, 42579, 42674, 42756, 42835, 42904, 42960, 43018, 43064, 43108, 43151, 43188, 43226, 43259, 43287, 43318, 43340, 43366, 43390, 43410, 43428, 43446, 43463, 43478, 43499, 43512, 43529, 43540, 43555, 43566, 43577, 43587, 43599, 43606, 43613, 43623, 43635, 43639, 43645, 43650, 43660, 43666, 43673, 43684, 43690, 43696, 43702, 43708, 43716, 43720, 43728, 43727

Top punched card per input:
Training results: 46145 correct recognitions of 60000
Test results: 46155 correct recognitions of 60000

Top 1 (5%) punched cards per input:
Training results: 46145 correct recognitions of 60000
Test results: 46155 correct recognitions of 60000

All punched cards:
Training results: 52459 correct recognitions of 60000
Test results: 52233 correct recognitions of 60000

Press "Enter" to exit the program...
```
