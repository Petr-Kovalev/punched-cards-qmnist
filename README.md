### 'Punched cards' for QMNIST

*Object recognition by sparse random binary data lookup. Based on [this article](https://petr-kovalev.medium.com/punched-cards-object-recognition-97523a98857b)*

Performing single-shot QMNIST handwritten digits recognition by lookup over the most different sparse input bit sets of the training data (out of 28⋅28⋅8 = 6272 bits per training sample)

Same algorithm applied to the Fashion-MNIST dataset [is here](https://github.com/Petr-Kovalev/punched-cards-fashion-mnist)

### Program output:
```
Punched card bit length: 8

Global top punched card:
Unique input combinations per punched card (descending): {173, 164, 131, 127, 126, 126, 124, 114, 110, 34: sum 1229}: total sum 1229
Training results: 19577 correct recognitions of 60000
Test results: 19638 correct recognitions of 60000

Top punched cards per label:
Unique input combinations per punched card (descending): {211, 188, 175, 171, 169, 141, 125, 112, 88: sum 1380}, {173, 164, 131, 127, 126, 126, 124, 114, 110: sum 1195}, {206, 139, 130, 126, 122, 116, 112, 102, 73, 60: sum 1186}, {128, 122, 119, 105, 99, 97, 91, 86, 84: sum 931}, {128, 110, 108, 98, 94, 93, 81, 78, 64: sum 854}, {118, 115, 106, 105, 100, 92, 80, 70, 44: sum 830}, {148, 135, 134, 109, 108, 87, 43: sum 764}, {122, 97, 93, 81, 80, 77, 69, 68, 45: sum 732}, {107, 96, 78, 69, 65, 64, 61, 58, 47, 44: sum 689}, {111, 96, 88, 86, 81, 80, 65, 58: sum 665}, {105, 93, 88, 83, 78, 75, 74, 66: sum 662}, {125, 124, 120, 112, 103, 45: sum 629}, {109, 102, 93, 87, 86, 78, 35, 31: sum 621}, {140, 138, 132, 129, 74: sum 613}, {106, 94, 88, 81, 79, 78, 77: sum 603}, {87, 80, 67, 66, 66, 64, 63, 61, 34: sum 588}, {102, 94, 80, 72, 69, 68, 67: sum 552}, {84, 74, 65, 63, 61, 56, 56, 55, 37: sum 551}, {101, 99, 85, 70, 66, 60, 49: sum 530}, {125, 100, 90, 82, 82, 49: sum 528}, {99, 94, 71, 67, 65, 53, 52: sum 501}, {79, 75, 74, 69, 67, 63, 46, 28: sum 501}, {117, 113, 98, 87, 84: sum 499}, {96, 69, 63, 61, 55, 54, 51, 31: sum 480}, {96, 95, 92, 85, 75, 28: sum 471}, {92, 68, 63, 63, 62, 61, 61: sum 470}, {93, 86, 71, 65, 58, 48, 43: sum 464}, {136, 111, 98, 87: sum 432}, {86, 86, 82, 78, 68, 32: sum 432}, {81, 81, 78, 74, 54, 48: sum 416}, {100, 96, 93, 82, 36: sum 407}, {88, 76, 63, 56, 44, 43: sum 370}, {164, 131, 67: sum 362}, {77, 73, 70, 65, 48, 27: sum 360}, {76, 64, 64, 60, 48, 47: sum 359}, {95, 93, 63, 56, 48: sum 355}, {66, 62, 62, 58, 53, 51: sum 352}, {76, 66, 51, 49, 41, 32, 31: sum 346}, {68, 63, 62, 59, 59, 32: sum 343}, {75, 71, 66, 59, 38, 30: sum 339}, {108, 106, 70, 54: sum 338}, {91, 84, 69, 60, 28: sum 332}, {85, 64, 56, 51, 40, 35: sum 331}, {98, 86, 84, 56: sum 324}, {64, 53, 53, 44, 43, 34, 13: sum 304}, {48, 45, 44, 41, 32, 30, 30, 29: sum 299}, {56, 51, 50, 45, 36, 31, 22: sum 291}, {112, 90, 89: sum 291}, {88, 82, 62, 57: sum 289}, {110, 104, 69: sum 283}, {61, 55, 54, 52, 48: sum 270}, {76, 61, 54, 47, 31: sum 269}, {156, 111: sum 267}, {84, 82, 57, 41: sum 264}, {86, 84, 70, 20: sum 260}, {71, 66, 65, 56: sum 258}, {73, 65, 61, 59: sum 258}, {73, 62, 61, 60: sum 256}, {90, 85, 80: sum 255}, {63, 63, 63, 63: sum 252}, {69, 68, 54, 52: sum 243}, {72, 48, 44, 42, 37: sum 243}, {91, 71, 70: sum 232}, {127, 91: sum 218}, {32, 31, 31, 31, 31, 30, 30: sum 216}, {59, 50, 49, 32, 20: sum 210}, {63, 52, 51, 38: sum 204}, {57, 48, 47, 42: sum 194}, {50, 48, 36, 33, 27: sum 194}, {69, 66, 55: sum 190}, {55, 55, 38, 37: sum 185}, {96, 87: sum 183}, {55, 33, 33, 31, 31: sum 183}, {61, 61, 54: sum 176}, {79, 63, 34: sum 176}, {58, 44, 43, 26: sum 171}, {89, 49, 29: sum 167}, {69, 65, 33: sum 167}, {65, 56, 43: sum 164}, {61, 52, 50: sum 163}, {48, 41, 36, 31: sum 156}, {74, 45, 36: sum 155}, {49, 38, 35, 32: sum 154}, {67, 57, 28: sum 152}, {40, 36, 31, 24, 18: sum 149}, {55, 50, 43: sum 148}, {54, 46, 43: sum 143}, {56, 33, 25, 25: sum 139}, {42, 31, 31, 31: sum 135}, {68, 65: sum 133}, {73, 45, 15: sum 133}, {93, 38: sum 131}, {64, 63: sum 127}, {107, 19: sum 126}, {54, 50, 20: sum 124}, {63, 60: sum 123}, {43, 42, 32: sum 117}, {62, 48: sum 110}, {40, 35, 35: sum 110}, {61, 49: sum 110}, {48, 32, 25: sum 105}, {40, 38, 22: sum 100}, {56, 44: sum 100}, {61, 37: sum 98}, {50, 48: sum 98}, {60, 35: sum 95}, {91}, {90}, {90}, {46, 43: sum 89}, {31, 31, 25: sum 87}, {27, 24, 20, 16: sum 87}, {65, 18: sum 83}, {29, 28, 25: sum 82}, {46, 33: sum 79}, {44, 35: sum 79}, {38, 36: sum 74}, {20, 19, 19, 15: sum 73}, {38, 33: sum 71}, {53, 17: sum 70}, {36, 34: sum 70}, {20, 19, 15, 15: sum 69}, {32, 21, 16: sum 69}, {65}, {63}, {62}, {62}, {31, 31: sum 62}, {33, 28: sum 61}, {61}, {60}, {44, 13: sum 57}, {57}, {55}, {31, 23: sum 54}, {53}, {31, 21: sum 52}, {52}, {40, 10: sum 50}, {50}, {49}, {48}, {28, 20: sum 48}, {34, 13: sum 47}, {45}, {45}, {45}, {44}, {42}, {26, 16: sum 42}, {40}, {39}, {25, 12: sum 37}, {22, 15: sum 37}, {36}, {36}, {34}, {33}, {33}, {33}, {33}, {32}, {32}, {31}, {31}, {31}, {31}, {31}, {31}, {31}, {17, 13: sum 30}, {30}, {29}, {28}, {27}, {27}, {26}, {26}, {24}, {23}, {23}, {21}, {19}, {19}, {18}, {18}, {15}, {15}, {14}, {14}, {10}, {10}: total sum 40125
Training results: 42323 correct recognitions of 60000
Test results: 42269 correct recognitions of 60000

Punched card bit length: 16

Global top punched card:
Unique input combinations per punched card (descending): {1122, 914, 862, 703, 696, 687, 407, 349, 266, 230: sum 6236}: total sum 6236
Training results: 16558 correct recognitions of 60000
Test results: 16394 correct recognitions of 60000

Top punched cards per label:
Unique input combinations per punched card (descending): {1381, 1152, 1150, 941, 755, 692, 676, 608: sum 7355}, {1088, 1087, 1073, 957, 793, 566, 537, 450: sum 6551}, {1024, 788, 768, 759, 678, 639, 635, 525, 437, 105: sum 6358}, {969, 947, 744, 729, 718, 633, 610, 479: sum 5829}, {884, 785, 734, 641, 560, 558, 522, 519, 371, 148: sum 5722}, {875, 845, 748, 682, 641, 564, 501, 443, 155: sum 5454}, {779, 771, 682, 640, 572, 512, 499, 492, 446: sum 5393}, {1106, 851, 596, 586, 578, 544, 394, 295, 254, 145: sum 5349}, {1122, 914, 862, 703, 696, 687, 230: sum 5214}, {790, 738, 567, 561, 514, 486, 406, 366, 334, 160: sum 4922}, {1068, 1030, 819, 703, 544, 328: sum 4492}, {996, 976, 791, 601, 499, 475: sum 4338}, {806, 646, 640, 625, 623, 481, 376: sum 4197}, {712, 613, 601, 568, 459, 438, 347, 215: sum 3953}, {1084, 1012, 913, 716: sum 3725}, {737, 615, 609, 547, 430, 386, 330: sum 3654}, {745, 544, 536, 509, 469, 464, 321: sum 3588}, {575, 556, 533, 496, 432, 280, 256, 165, 107: sum 3400}, {653, 573, 566, 552, 527, 391, 136: sum 3398}, {470, 444, 426, 392, 391, 390, 387, 341, 157: sum 3398}, {711, 694, 671, 594, 594: sum 3264}, {904, 512, 486, 481, 455, 408: sum 3246}, {600, 578, 548, 545, 485, 295, 158: sum 3209}, {624, 596, 482, 442, 419, 383, 217: sum 3163}, {577, 467, 456, 435, 416, 415, 308, 86: sum 3160}, {620, 530, 514, 510, 479, 436: sum 3089}, {484, 479, 457, 457, 421, 394, 386: sum 3078}, {740, 642, 563, 454, 376, 296: sum 3071}, {398, 387, 380, 375, 320, 319, 311, 283, 281: sum 3054}, {452, 425, 402, 391, 351, 336, 327, 213, 117: sum 3014}, {805, 595, 591, 473, 455: sum 2919}, {600, 457, 424, 376, 358, 345, 250, 97: sum 2907}, {739, 575, 411, 372, 247, 193, 181: sum 2718}, {839, 635, 406, 403, 394: sum 2677}, {466, 377, 374, 366, 348, 332, 283, 102: sum 2648}, {581, 514, 476, 469, 262, 231, 95: sum 2628}, {441, 431, 427, 401, 376, 259, 187, 101: sum 2623}, {550, 520, 494, 476, 308, 252: sum 2600}, {645, 520, 493, 314, 299, 273: sum 2544}, {577, 557, 496, 462, 374: sum 2466}, {553, 495, 465, 426, 390, 102: sum 2431}, {709, 609, 572, 534: sum 2424}, {722, 685, 679, 251: sum 2337}, {676, 458, 417, 413, 355: sum 2319}, {558, 558, 386, 328, 298: sum 2128}, {574, 526, 517, 507: sum 2124}, {606, 488, 352, 290, 259: sum 1995}, {561, 367, 305, 239, 222, 217, 82: sum 1993}, {307, 297, 287, 285, 274, 251, 251: sum 1952}, {406, 359, 355, 277, 277, 257: sum 1931}, {773, 580, 479: sum 1832}, {438, 345, 250, 224, 218, 183, 91: sum 1749}, {529, 499, 397, 313: sum 1738}, {521, 458, 392, 211, 142: sum 1724}, {461, 454, 446, 353: sum 1714}, {278, 246, 223, 208, 206, 175, 175, 82: sum 1593}, {405, 369, 322, 253, 229: sum 1578}, {548, 435, 241, 234, 101: sum 1559}, {619, 496, 392: sum 1507}, {311, 293, 270, 261, 203, 167: sum 1505}, {979, 500: sum 1479}, {323, 299, 257, 244, 235, 74: sum 1432}, {509, 407, 283, 138, 87: sum 1424}, {704, 701: sum 1405}, {356, 280, 266, 254, 214: sum 1370}, {506, 386, 301, 93: sum 1286}, {640, 567, 76: sum 1283}, {435, 428, 382: sum 1245}, {378, 370, 362, 118: sum 1228}, {454, 263, 256, 242: sum 1215}, {386, 356, 240, 208: sum 1190}, {584, 483, 100: sum 1167}, {785, 375: sum 1160}, {317, 304, 249, 180, 79: sum 1129}, {850, 268: sum 1118}, {370, 325, 308, 95: sum 1098}, {394, 386, 274: sum 1054}, {563, 373, 83: sum 1019}, {329, 319, 276, 80: sum 1004}, {246, 241, 236, 211, 55: sum 989}, {518, 436: sum 954}, {951}, {353, 234, 197, 167: sum 951}, {529, 408: sum 937}, {308, 262, 248, 101: sum 919}, {212, 192, 174, 161, 113, 57: sum 909}, {362, 293, 237: sum 892}, {423, 390, 78: sum 891}, {489, 388: sum 877}, {347, 245, 206, 62: sum 860}, {432, 349, 57: sum 838}, {303, 278, 254: sum 835}, {377, 231, 205: sum 813}, {406, 379: sum 785}, {257, 256, 246: sum 759}, {465, 198, 85: sum 748}, {745}, {227, 221, 220, 70: sum 738}, {586, 139: sum 725}, {429, 287: sum 716}, {249, 249, 217: sum 715}, {221, 210, 140, 133: sum 704}, {392, 310: sum 702}, {312, 262, 71: sum 645}, {643}, {638}, {330, 296: sum 626}, {351, 258: sum 609}, {603}, {299, 266: sum 565}, {280, 164, 108: sum 552}, {233, 177, 141: sum 551}, {534}, {291, 242: sum 533}, {312, 218: sum 530}, {527}, {434, 88: sum 522}, {424, 96: sum 520}, {252, 160, 102: sum 514}, {305, 188: sum 493}, {461}, {420}, {226, 183: sum 409}, {400}, {396}, {211, 184: sum 395}, {390}, {387}, {116, 110, 109, 44: sum 379}, {196, 161: sum 357}, {349}, {241, 96: sum 337}, {172, 159: sum 331}, {320}, {316}, {214, 99: sum 313}, {312}, {244, 44: sum 288}, {285}, {283}, {282}, {188, 89: sum 277}, {276}, {268}, {130, 124: sum 254}, {252}, {248}, {247}, {228}, {222}, {163, 51: sum 214}, {212}, {208}, {183}, {180}, {179}, {179}, {177}, {175}, {163}, {159}, {158}, {127}, {107}, {105}, {102}, {94}, {91}, {90}, {66}, {63}, {59}, {55}, {53}, {53}, {49}, {43}, {37}: total sum 258428
Training results: 44084 correct recognitions of 60000
Test results: 44193 correct recognitions of 60000

Punched card bit length: 32

Global top punched card:
Unique input combinations per punched card (descending): {4824, 4750, 4378, 4038, 3952, 3838, 3596, 3471, 2931, 571: sum 36349}: total sum 36349
Training results: 25854 correct recognitions of 60000
Test results: 26148 correct recognitions of 60000

Top punched cards per label:
Unique input combinations per punched card (descending): {4793, 4737, 4725, 4667, 4005, 3903, 3674, 3617, 3387, 958: sum 38466}, {4824, 4750, 4378, 4038, 3952, 3838, 3596, 3471, 2931, 571: sum 36349}, {4390, 4052, 3824, 3377, 3154, 2950, 2942, 2860, 2819, 962: sum 31330}, {4590, 4476, 4324, 3752, 3626, 3455, 3026, 2923, 853: sum 31025}, {5038, 4986, 4863, 4516, 4441, 3139, 2701, 908: sum 30592}, {4167, 4154, 3900, 3701, 3463, 3433, 3358, 3202, 775: sum 30153}, {5041, 4137, 3975, 3704, 3687, 2921, 2578, 1942, 1887: sum 29872}, {3629, 3537, 3396, 3190, 3153, 3062, 2981, 2240, 2225: sum 27413}, {4032, 3850, 3781, 3608, 2974, 2585, 2339, 1931, 1478, 494: sum 27072}, {3731, 3517, 3450, 3353, 3049, 3037, 2407, 2159, 1912, 454: sum 27069}, {4595, 3839, 3077, 2949, 2946, 2468, 2401, 1903, 1708, 941: sum 26827}, {4516, 3757, 3535, 3161, 3147, 3052, 3040, 2157, 417: sum 26782}, {4191, 3647, 3496, 3323, 3175, 3034, 2765, 2251, 740: sum 26622}, {3629, 3618, 3227, 2980, 2753, 2616, 2560, 2400, 2289, 388: sum 26460}, {4126, 4005, 3468, 3433, 3175, 2198, 1921, 1887, 1598: sum 25811}, {4614, 3631, 3290, 3278, 3198, 2756, 2621, 2321: sum 25709}, {4215, 3918, 3890, 3715, 3431, 2844, 2165, 786: sum 24964}, {4111, 3919, 3472, 3230, 2937, 2814, 2090, 1556, 472: sum 24601}, {3686, 3123, 3119, 2928, 2614, 2495, 2294, 1822, 1293, 453: sum 23827}, {3636, 3504, 3397, 2984, 2969, 2476, 2247, 1330, 1089: sum 23632}, {3484, 3423, 3307, 3199, 2719, 2698, 1685, 1552, 1274: sum 23341}, {4056, 4020, 3216, 2991, 2980, 2753, 2702, 448: sum 23166}, {4349, 3983, 3799, 3779, 3310, 3041, 529: sum 22790}, {3486, 3255, 2980, 2796, 2780, 2629, 2215, 2144, 452: sum 22737}, {4287, 4005, 3677, 3283, 2203, 2029, 2002, 770: sum 22256}, {3719, 3518, 3204, 2764, 2291, 2259, 1916, 1807: sum 21478}, {3631, 3193, 3129, 2972, 2481, 1660, 1479, 1413, 1091, 295: sum 21344}, {3178, 3088, 2755, 2658, 2649, 2522, 2134, 1960, 359: sum 21303}, {4414, 3051, 2798, 2611, 2377, 2345, 1761, 1727: sum 21084}, {3565, 3483, 2881, 2326, 2239, 2135, 2002, 1429, 517: sum 20577}, {3778, 3612, 3563, 2734, 2178, 2070, 2021: sum 19956}, {4535, 3472, 2729, 2632, 2543, 2445, 872: sum 19228}, {4134, 3225, 3145, 2840, 2186, 1805, 1491, 387: sum 19213}, {3783, 2980, 2901, 2807, 2755, 1535, 1477, 309: sum 18547}, {3332, 3012, 2648, 2283, 1990, 1960, 1796, 1347: sum 18368}, {3525, 3390, 3361, 2579, 2361, 1526, 1490: sum 18232}, {3234, 2609, 2382, 2186, 2138, 2101, 1878, 1473, 201: sum 18202}, {4197, 3725, 3068, 3007, 2674, 1430: sum 18101}, {3687, 3249, 3002, 2814, 2617, 2304, 385: sum 18058}, {3452, 3362, 2699, 2643, 2531, 2257, 772: sum 17716}, {4057, 4031, 3597, 2967, 1844, 709: sum 17205}, {3425, 3211, 3160, 2581, 2251, 2000: sum 16628}, {4552, 4158, 3623, 2860, 1408: sum 16601}, {3168, 2496, 2374, 2239, 2032, 2013, 1417, 671: sum 16410}, {2964, 2571, 2268, 2195, 2015, 1930, 1898: sum 15841}, {2882, 2543, 2367, 2281, 2060, 1809, 1726: sum 15668}, {3235, 2655, 2460, 2454, 2304, 2122, 438: sum 15668}, {3861, 3140, 2212, 2194, 2049, 1430, 461: sum 15347}, {3326, 2635, 2469, 2272, 2176, 1421, 648: sum 14947}, {3319, 3253, 2295, 2155, 2003, 1837: sum 14862}, {3120, 2988, 2890, 2143, 1972, 1468, 204: sum 14785}, {2849, 2839, 2387, 2358, 2194, 2133: sum 14760}, {3163, 2686, 2298, 2096, 1807, 1610, 827: sum 14487}, {2982, 2726, 2591, 2096, 1944, 1919: sum 14258}, {3661, 2926, 2723, 2379, 1931, 398: sum 14018}, {2699, 2684, 2646, 2414, 2226, 1252: sum 13921}, {3159, 2800, 2774, 2427, 1551, 421: sum 13132}, {3789, 3444, 2223, 2062, 1429: sum 12947}, {2175, 2039, 1983, 1962, 1785, 1731, 256: sum 11931}, {3034, 2392, 2128, 1931, 1775: sum 11260}, {3254, 2280, 2244, 1933, 1540: sum 11251}, {2732, 2399, 2031, 1857, 1728, 333: sum 11080}, {3669, 2277, 2065, 1709, 1337: sum 11057}, {3237, 2247, 1971, 1737, 1237: sum 10429}, {3616, 2485, 2458, 1666: sum 10225}, {4391, 3141, 2575: sum 10107}, {2008, 1974, 1945, 1894, 1587: sum 9408}, {3057, 2471, 2170, 1180, 404: sum 9282}, {2829, 2503, 2020, 1473, 321: sum 9146}, {2880, 2187, 2030, 1992: sum 9089}, {2922, 2438, 1733, 1304, 357: sum 8754}, {3126, 2336, 1914, 489: sum 7865}, {3057, 1734, 1594, 1438: sum 7823}, {2274, 2015, 2002, 1320: sum 7611}, {2901, 2196, 2182: sum 7279}, {3345, 3086, 518: sum 6949}, {2998, 2542, 978, 328: sum 6846}, {2652, 2125, 1865: sum 6642}, {2828, 1811, 1288: sum 5927}, {2887, 2873: sum 5760}, {2815, 2457, 472: sum 5744}, {2768, 2489, 386: sum 5643}, {3000, 2274, 336: sum 5610}, {2401, 1780, 1385: sum 5566}, {3206, 1920, 389: sum 5515}, {2582, 1653, 1128: sum 5363}, {3116, 2223: sum 5339}, {3191, 1837, 296: sum 5324}, {2724, 2596: sum 5320}, {3124, 1950: sum 5074}, {1767, 1651, 1626: sum 5044}, {2809, 919, 833, 295: sum 4856}, {3205, 1431: sum 4636}, {2743, 1397, 412: sum 4552}, {2189, 2055: sum 4244}, {2113, 2025: sum 4138}, {2328, 1747: sum 4075}, {2016, 1891: sum 3907}, {2700, 1174: sum 3874}, {2108, 1746: sum 3854}, {1834, 1705, 201: sum 3740}, {3572}, {1844, 1704: sum 3548}, {3077, 270: sum 3347}, {2048, 1287: sum 3335}, {2869}, {2696}, {2475}, {2353}, {2319}, {1872, 362: sum 2234}, {1956}, {1830}, {1432, 275: sum 1707}, {1673}, {1519}, {1487}, {1271}, {1199}, {874}, {716}, {386}, {305}, {276}, {259}, {194}, {183}, {166}, {109}: total sum 1583755
Training results: 42562 correct recognitions of 60000
Test results: 42757 correct recognitions of 60000

Punched card bit length: 64

Global top punched card:
Unique input combinations per punched card (descending): {6113, 6102, 5913, 5901, 5862, 5849, 5835, 5753, 5398, 2335: sum 55061}: total sum 55061
Training results: 33668 correct recognitions of 60000
Test results: 33950 correct recognitions of 60000

Top punched cards per label:
Unique input combinations per punched card (descending): {6122, 5952, 5941, 5853, 5836, 5830, 5782, 5638, 5352, 3413: sum 55719}, {6113, 6102, 5913, 5901, 5862, 5849, 5835, 5753, 5398, 2335: sum 55061}, {6114, 5913, 5871, 5833, 5746, 5651, 5378, 5375, 5113, 3745: sum 54739}, {6006, 5935, 5873, 5870, 5832, 5758, 5605, 5565, 5341, 2636: sum 54421}, {6112, 5947, 5927, 5856, 5826, 5823, 5627, 5477, 5335, 2282: sum 54212}, {6066, 5844, 5835, 5746, 5698, 5587, 5486, 5374, 5223, 3279: sum 54138}, {5938, 5912, 5892, 5810, 5729, 5604, 5346, 5333, 5202, 2671: sum 53437}, {6040, 5929, 5912, 5818, 5765, 5753, 5625, 5593, 5349, 1540: sum 53324}, {6076, 5914, 5843, 5787, 5776, 5490, 5412, 5392, 5107, 2413: sum 53210}, {6025, 5839, 5802, 5781, 5779, 5721, 5563, 5310, 5174, 1663: sum 52657}, {6016, 5825, 5796, 5559, 5491, 5324, 5305, 5282, 5055, 2499: sum 52152}, {5955, 5950, 5911, 5823, 5802, 5697, 5690, 5687, 5333: sum 51848}, {5965, 5914, 5787, 5766, 5512, 5483, 5347, 5321, 5257, 1467: sum 51819}, {5934, 5870, 5756, 5679, 5672, 5435, 5363, 5106, 4804, 2012: sum 51631}, {5992, 5826, 5794, 5702, 5450, 5190, 5173, 5021, 4840, 2155: sum 51143}, {6114, 5892, 5871, 5813, 5597, 5526, 5477, 5011, 4714, 1099: sum 51114}, {5860, 5854, 5814, 5757, 5678, 5628, 5251, 5213, 4523, 1267: sum 50845}, {5830, 5778, 5763, 5673, 5389, 5268, 5257, 5079, 4967, 1830: sum 50834}, {5892, 5813, 5711, 5590, 5341, 5091, 5031, 5025, 4972, 1887: sum 50353}, {5847, 5774, 5740, 5730, 5274, 5244, 4895, 4733, 4578, 2098: sum 49913}, {5929, 5908, 5672, 5536, 5517, 5325, 5103, 4889, 4058, 1805: sum 49742}, {5905, 5864, 5851, 5620, 5275, 5197, 5012, 4625, 4600, 1757: sum 49706}, {5942, 5834, 5736, 5735, 5645, 5398, 5175, 5152, 4991: sum 49608}, {5874, 5871, 5844, 5752, 5310, 5307, 5279, 5101, 5069: sum 49407}, {5857, 5766, 5675, 5517, 5345, 5237, 5175, 5115, 4280, 1162: sum 49129}, {5826, 5753, 5738, 5553, 5383, 5300, 5193, 4497, 4481, 1223: sum 48947}, {5992, 5917, 5822, 5771, 5727, 5336, 5220, 5205, 2587: sum 47577}, {6003, 5859, 5816, 5782, 5532, 5455, 5333, 4831, 2746: sum 47357}, {5905, 5613, 5507, 5441, 5150, 5144, 4963, 4961, 4534: sum 47218}, {5825, 5737, 5522, 5499, 5485, 5430, 5108, 5000, 2691: sum 46297}, {5850, 5732, 5699, 5675, 5647, 5637, 5535, 5134, 1356: sum 46265}, {6044, 5919, 5741, 5326, 5179, 5164, 5150, 4950, 2229: sum 45702}, {5808, 5793, 5566, 5490, 5481, 5185, 5011, 4841, 2356: sum 45531}, {5881, 5827, 5669, 5589, 5431, 5335, 5208, 4688, 1807: sum 45435}, {5732, 5694, 5640, 5596, 5520, 5470, 5191, 4855, 1694: sum 45392}, {5954, 5855, 5583, 5543, 5514, 5481, 5062, 5011, 1330: sum 45333}, {5825, 5774, 5759, 5709, 5562, 5158, 4563, 4072, 2617: sum 45039}, {5924, 5835, 5753, 5607, 5541, 5258, 5113, 4438, 1538: sum 45007}, {6007, 5881, 5870, 5777, 5313, 5308, 5284, 5111: sum 44551}, {5878, 5742, 5667, 5320, 5278, 5255, 4901, 4582, 1211: sum 43834}, {5849, 5669, 5622, 5582, 5402, 5158, 5121, 4906: sum 43309}, {5868, 5825, 5604, 5554, 5102, 5041, 5000, 4616: sum 42610}, {5848, 5549, 5438, 5139, 5111, 5087, 4810, 4508, 1101: sum 42591}, {5959, 5793, 5551, 5376, 5202, 5182, 5025, 1453: sum 39541}, {5840, 5829, 5823, 5574, 5104, 5060, 4925, 1379: sum 39534}, {5905, 5844, 5577, 5506, 5135, 4956, 4256, 1291: sum 38470}, {5810, 5670, 5328, 5277, 5153, 5061, 4658, 1266: sum 38223}, {5586, 5569, 5408, 5402, 5018, 4911, 4897, 1217: sum 38008}, {5857, 5809, 5724, 5496, 5122, 4896, 4562: sum 37466}, {5650, 5329, 5295, 5094, 5031, 4970, 4786, 1045: sum 37200}, {5673, 5579, 5238, 5216, 5197, 5093, 4893: sum 36889}, {5851, 5755, 5737, 4795, 4794, 4789, 4459: sum 36180}, {5857, 5769, 5726, 5340, 5247, 5140, 2536: sum 35615}, {5647, 5617, 4857, 4832, 4765, 4652, 4651: sum 35021}, {5761, 5683, 5245, 5145, 5132, 5102, 1613: sum 33681}, {5970, 5820, 5652, 5108, 4484, 3995, 1706: sum 32735}, {5605, 5588, 5134, 4975, 4865, 4846, 1183: sum 32196}, {5818, 5702, 5570, 5407, 5023, 4511: sum 32031}, {5851, 5746, 5522, 5459, 4739, 4657: sum 31974}, {5631, 5541, 5512, 4573, 4520, 4434, 1479: sum 31690}, {5954, 5883, 5650, 4922, 4769, 1451: sum 28629}, {5720, 5555, 5491, 5034, 4917, 1655: sum 28372}, {5803, 5718, 5444, 5165, 4482, 1355: sum 27967}, {5702, 5583, 5341, 4869, 4767, 1452: sum 27714}, {5931, 5595, 4749, 4134, 3590, 3533: sum 27532}, {5807, 5790, 5465, 5453, 1414: sum 23929}, {5336, 5280, 4367, 3962, 3798, 1064: sum 23807}, {6000, 5613, 5362, 5008, 1812: sum 23795}, {5650, 5480, 5086, 5038, 1285: sum 22539}, {5677, 5449, 4943, 4643, 1737: sum 22449}, {5731, 5354, 5076, 4957: sum 21118}, {5791, 5573, 4519, 3862: sum 19745}, {5722, 5139, 4802, 1613: sum 17276}, {5451, 4850, 4732, 2228: sum 17261}, {5896, 5087, 4793, 1153: sum 16929}, {5754, 5636, 5276: sum 16666}, {5786, 5748, 5126: sum 16660}, {5439, 5426, 4776: sum 15641}, {5535, 4941, 4796: sum 15272}, {5503, 4994, 4436: sum 14933}, {5395, 5028, 1319: sum 11742}, {5576, 3836, 1726: sum 11138}, {5608, 5457: sum 11065}, {5624, 5327: sum 10951}, {5599}, {5562}, {4818}, {4676}, {4551}, {3304, 966: sum 4270}, {1396}, {708}: total sum 3229321
Training results: 42181 correct recognitions of 60000
Test results: 42483 correct recognitions of 60000

Punched card bit length: 128

Global top punched card:
Unique input combinations per punched card (descending): {6471, 6262, 6131, 5958, 5947, 5923, 5918, 5851, 5842, 5421: sum 59724}: total sum 59724
Training results: 36966 correct recognitions of 60000
Test results: 36994 correct recognitions of 60000

Top punched cards per label:
Unique input combinations per punched card (descending): {6489, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5841, 5420: sum 59745}, {6471, 6262, 6131, 5958, 5947, 5923, 5918, 5851, 5842, 5421: sum 59724}, {6441, 6237, 6131, 5958, 5937, 5923, 5917, 5851, 5824, 5421: sum 59640}, {6322, 6252, 6131, 5958, 5943, 5923, 5917, 5851, 5839, 5421: sum 59557}, {6300, 6265, 6131, 5958, 5948, 5923, 5913, 5851, 5842, 5421: sum 59552}, {6289, 6250, 6131, 5958, 5944, 5923, 5916, 5851, 5842, 5421: sum 59525}, {6264, 6192, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59449}, {6263, 6131, 6069, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59325}, {6253, 6131, 5989, 5958, 5948, 5923, 5917, 5851, 5841, 5421: sum 59232}, {6261, 6128, 5958, 5949, 5943, 5923, 5915, 5851, 5842, 5418: sum 59188}, {6265, 6131, 5958, 5949, 5923, 5918, 5909, 5851, 5842, 5421: sum 59167}, {6257, 6131, 5958, 5947, 5923, 5918, 5851, 5840, 5794, 5420: sum 59039}, {6262, 6131, 5958, 5948, 5923, 5918, 5851, 5842, 5684, 5421: sum 58938}, {6263, 6131, 5958, 5949, 5922, 5918, 5851, 5842, 5665, 5421: sum 58920}, {6247, 6129, 5958, 5941, 5922, 5911, 5851, 5841, 5676, 5421: sum 58897}, {6253, 6129, 5957, 5947, 5922, 5907, 5851, 5839, 5650, 5421: sum 58876}, {6230, 6131, 5958, 5924, 5922, 5908, 5850, 5830, 5526, 5421: sum 58700}, {6246, 6131, 5958, 5948, 5921, 5916, 5851, 5842, 5421, 5412: sum 58646}, {6250, 6131, 5958, 5948, 5923, 5916, 5851, 5837, 5421, 5366: sum 58601}, {6264, 6130, 5958, 5946, 5923, 5915, 5851, 5842, 5420, 5348: sum 58597}, {6261, 6131, 5958, 5949, 5923, 5917, 5851, 5842, 5420, 5310: sum 58562}, {6257, 6131, 5958, 5945, 5923, 5917, 5851, 5835, 5418, 5266: sum 58501}, {6259, 6131, 5958, 5949, 5923, 5916, 5850, 5842, 5420, 5242: sum 58490}, {6248, 6131, 5958, 5940, 5923, 5913, 5851, 5839, 5421, 5223: sum 58447}, {6215, 6131, 5958, 5944, 5923, 5913, 5849, 5841, 5420, 5242: sum 58436}, {6220, 6131, 5958, 5940, 5923, 5916, 5851, 5839, 5421, 4993: sum 58192}, {6258, 6131, 5958, 5943, 5923, 5903, 5851, 5833, 5419, 4926: sum 58145}, {6261, 6130, 5958, 5945, 5922, 5911, 5851, 5840, 5420, 4837: sum 58075}, {6126, 6117, 5958, 5925, 5923, 5916, 5851, 5835, 5420, 4966: sum 58037}, {6235, 6131, 5958, 5947, 5922, 5894, 5851, 5832, 5417, 4751: sum 57938}, {6250, 6130, 5958, 5935, 5919, 5918, 5849, 5836, 5387, 4463: sum 57645}, {6150, 6125, 5958, 5934, 5923, 5916, 5849, 5842, 5421, 4374: sum 57492}, {6264, 6131, 5958, 5946, 5923, 5904, 5851, 5842, 5421, 4223: sum 57463}, {6243, 6129, 5954, 5947, 5921, 5915, 5851, 5840, 5420, 4110: sum 57330}, {6128, 6094, 5958, 5923, 5922, 5912, 5851, 5824, 5418, 4296: sum 57326}, {6254, 6129, 5952, 5933, 5922, 5914, 5851, 5840, 5420, 4087: sum 57302}, {6262, 6124, 5956, 5923, 5918, 5895, 5849, 5832, 5418, 4066: sum 57243}, {6222, 6128, 5958, 5926, 5923, 5913, 5851, 5837, 5419, 4051: sum 57228}, {6237, 6130, 5957, 5944, 5923, 5918, 5851, 5841, 5421, 3991: sum 57213}, {6231, 6129, 5956, 5924, 5918, 5874, 5848, 5804, 5419, 4043: sum 57146}, {6190, 6130, 5958, 5923, 5914, 5858, 5850, 5815, 5417, 3995: sum 57050}, {6131, 6087, 5957, 5923, 5905, 5847, 5830, 5825, 5421, 3956: sum 56882}, {6130, 5958, 5938, 5923, 5913, 5848, 5837, 5756, 5419, 4133: sum 56855}, {6227, 6127, 5956, 5938, 5922, 5909, 5849, 5835, 5419, 3571: sum 56753}, {6129, 6125, 5958, 5922, 5915, 5885, 5851, 5829, 5420, 3338: sum 56372}, {6093, 6007, 5957, 5920, 5909, 5845, 5779, 5632, 5402, 3455: sum 55999}, {6184, 6124, 5958, 5922, 5870, 5864, 5835, 5760, 5420, 2678: sum 55615}, {6210, 6120, 5956, 5933, 5923, 5911, 5849, 5834, 5417, 2458: sum 55611}, {6096, 6060, 5953, 5923, 5912, 5890, 5848, 5817, 5418, 2599: sum 55516}: total sum 2846182
Training results: 41769 correct recognitions of 60000
Test results: 42144 correct recognitions of 60000

Punched card bit length: 256

Global top punched card:
Unique input combinations per punched card (descending): {6730, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59988}: total sum 59988
Training results: 40065 correct recognitions of 60000
Test results: 40533 correct recognitions of 60000

Top punched cards per label:
Unique input combinations per punched card (descending): {6740, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59998}, {6737, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59995}, {6737, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59995}, {6737, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59995}, {6736, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59994}, {6735, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59993}, {6734, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59992}, {6733, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59991}, {6730, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59988}, {6730, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59988}, {6728, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59986}, {6719, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59977}, {6717, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59975}, {6716, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59974}, {6712, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59970}, {6706, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59964}, {6700, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59958}, {6699, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59957}, {6693, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59951}, {6686, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59944}, {6686, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59944}, {6673, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59931}, {6653, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59911}, {6593, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59851}: total sum 1439222
Training results: 41779 correct recognitions of 60000
Test results: 42113 correct recognitions of 60000

Press "Enter" to exit the program...
```