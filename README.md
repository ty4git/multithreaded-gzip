# multithreaded-gzip

You can get example.txt file here
	https://stackoverflow.com/questions/44492576/looking-for-large-text-files-for-testing-compression-in-all-sizes
	http://mattmahoney.net/dc/textdata.html

I used an enwik8 from http://mattmahoney.net/dc/textdata.html

# Stream compression. Single thread.

buffer size: 8192 B = 8 KB = 0.008 MB
example.txt
total time: 3366.5956 ms = 3.3665956 s
total time: min:sec.ms: 00:03.366
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 29703597.605 B/s = 29007.42 KB/s = 28.327 MB/s

buffer size: 16384 B = 16 KB = 0.016 MB
example.txt
total time: 3293.4411 ms = 3.2934411 s
total time: min:sec.ms: 00:03.293
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 30363379.202 B/s = 29651.738 KB/s = 28.957 MB/s

buffer size: 32768 B = 32 KB = 0.031 MB
example.txt
total time: 3128.7336 ms = 3.1287336 s
total time: min:sec.ms: 00:03.128
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 31961813.879 B/s = 31212.709 KB/s = 30.481 MB/s

buffer size: 65536 B = 64 KB = 0.062 MB
example.txt
total time: 3150.9328 ms = 3.1509328 s
total time: min:sec.ms: 00:03.150
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 31736633.99 B/s = 30992.807 KB/s = 30.266 MB/s

buffer size: 131072 B = 128 KB = 0.125 MB
example.txt
total time: 3139.9236 ms = 3.1399236 s
total time: min:sec.ms: 00:03.139
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 31847908.975 B/s = 31101.474 KB/s = 30.372 MB/s

buffer size: 262144 B = 256 KB = 0.25 MB
example.txt
total time: 3111.0022 ms = 3.1110022 s
total time: min:sec.ms: 00:03.111
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 32143982.733 B/s = 31390.608 KB/s = 30.655 MB/s

buffer size: 524288 B = 512 KB = 0.5 MB
example.txt
total time: 3109.236 ms = 3.109236 s
total time: min:sec.ms: 00:03.109
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 32162242.107 B/s = 31408.44 KB/s = 30.672 MB/s

buffer size: 1048576 B = 1024 KB = 1 MB
example.txt
total time: 3163.4842 ms = 3.1634842 s
total time: min:sec.ms: 00:03.163
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 31610716.121 B/s = 30869.84 KB/s = 30.146 MB/s


# Stream compression. Multiple threads via linq.

buffer size: 8192 B = 8 KB = 0.008 MB
example.txt
total time: 948.7096 ms = 0.9487096 s
total time: min:sec.ms: 00:00.948
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 105406334.035 B/s = 102935.873 KB/s = 100.523 MB/s

buffer size: 16384 B = 16 KB = 0.016 MB
example.txt
total time: 749.6195 ms = 0.7496195 s
total time: min:sec.ms: 00:00.749
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 133401013.447 B/s = 130274.427 KB/s = 127.221 MB/s

buffer size: 32768 B = 32 KB = 0.031 MB
example.txt
total time: 795.0265 ms = 0.7950265 s
total time: min:sec.ms: 00:00.795
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 125781972.047 B/s = 122833.957 KB/s = 119.954 MB/s

buffer size: 65536 B = 64 KB = 0.062 MB
example.txt
total time: 839.0231 ms = 0.8390231 s
total time: min:sec.ms: 00:00.839
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 119186230.987 B/s = 116392.804 KB/s = 113.664 MB/s

buffer size: 131072 B = 128 KB = 0.125 MB
example.txt
total time: 866.9999 ms = 0.8669999 s
total time: min:sec.ms: 00:00.866
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 115340268.205 B/s = 112636.981 KB/s = 109.997 MB/s

buffer size: 262144 B = 256 KB = 0.25 MB
example.txt
total time: 832.5103 ms = 0.8325103 s
total time: min:sec.ms: 00:00.832
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 120118635.169 B/s = 117303.355 KB/s = 114.554 MB/s

buffer size: 524288 B = 512 KB = 0.5 MB
example.txt
total time: 864.4221 ms = 0.8644221 s
total time: min:sec.ms: 00:00.864
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 115684225.334 B/s = 112972.876 KB/s = 110.325 MB/s

buffer size: 1048576 B = 1024 KB = 1 MB
example.txt
total time: 846.1583 ms = 0.8461583 s
total time: min:sec.ms: 00:00.846
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 118181197.301 B/s = 115411.326 KB/s = 112.706 MB/s


# Stream compression. Multiple threads custom implementation via thread pool with tasks.

buffer size: 8192 B = 8 KB = 0.008 MB
example.txt
total time: 1149.3198 ms = 1.1493198 s
total time: min:sec.ms: 00:01.149
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 87007985.941 B/s = 84968.736 KB/s = 82.977 MB/s

buffer size: 16384 B = 16 KB = 0.016 MB
example.txt
total time: 822.8119 ms = 0.8228119 s
total time: min:sec.ms: 00:00.822
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 121534461.278 B/s = 118685.997 KB/s = 115.904 MB/s

buffer size: 32768 B = 32 KB = 0.031 MB
example.txt
total time: 821.3861 ms = 0.8213861 s
total time: min:sec.ms: 00:00.821
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 121745426.42 B/s = 118892.018 KB/s = 116.105 MB/s

buffer size: 65536 B = 64 KB = 0.062 MB
example.txt
total time: 849.5952 ms = 0.8495952 s
total time: min:sec.ms: 00:00.849
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 117703114.377 B/s = 114944.448 KB/s = 112.25 MB/s

buffer size: 131072 B = 128 KB = 0.125 MB
example.txt
total time: 901.7574 ms = 0.9017574 s
total time: min:sec.ms: 00:00.901
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 110894572.088 B/s = 108295.481 KB/s = 105.757 MB/s

buffer size: 262144 B = 256 KB = 0.25 MB
example.txt
total time: 937.8458 ms = 0.9378458 s
total time: min:sec.ms: 00:00.937
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 106627337.884 B/s = 104128.26 KB/s = 101.687 MB/s

buffer size: 524288 B = 512 KB = 0.5 MB
example.txt
total time: 971.3654 ms = 0.9713654 s
total time: min:sec.ms: 00:00.971
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 102947872.14 B/s = 100535.031 KB/s = 98.178 MB/s

buffer size: 1048576 B = 1024 KB = 1 MB
example.txt
total time: 958.0601 ms = 0.9580601 s
total time: min:sec.ms: 00:00.958
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 104377586.542 B/s = 101931.237 KB/s = 99.542 MB/s


# Stream compression. Multiple threads custom implementation via static threads.

buffer size: 8192 B = 8 KB = 0.008 MB
example.txt
total time: 899.7182 ms = 0.8997182 s
total time: min:sec.ms: 00:00.899
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 111145913.243 B/s = 108540.931 KB/s = 105.997 MB/s

buffer size: 16384 B = 16 KB = 0.016 MB
example.txt
total time: 767.9444 ms = 0.7679444 s
total time: min:sec.ms: 00:00.767
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 130217761.859 B/s = 127165.783 KB/s = 124.185 MB/s

buffer size: 32768 B = 32 KB = 0.031 MB
example.txt
total time: 772.8911 ms = 0.7728911 s
total time: min:sec.ms: 00:00.772
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 129384334.999 B/s = 126351.89 KB/s = 123.39 MB/s

buffer size: 65536 B = 64 KB = 0.062 MB
example.txt
total time: 863.3138 ms = 0.8633138 s
total time: min:sec.ms: 00:00.863
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 115832737.76 B/s = 113117.908 KB/s = 110.466 MB/s

buffer size: 131072 B = 128 KB = 0.125 MB
example.txt
total time: 842.9325 ms = 0.8429325 s
total time: min:sec.ms: 00:00.842
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 118633462.347 B/s = 115852.991 KB/s = 113.137 MB/s

buffer size: 262144 B = 256 KB = 0.25 MB
example.txt
total time: 866.2344 ms = 0.8662344 s
total time: min:sec.ms: 00:00.866
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 115442195.554 B/s = 112736.519 KB/s = 110.094 MB/s

buffer size: 524288 B = 512 KB = 0.5 MB
example.txt
total time: 856.4965 ms = 0.8564965 s
total time: min:sec.ms: 00:00.856
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 116754710.615 B/s = 114018.272 KB/s = 111.345 MB/s

buffer size: 1048576 B = 1024 KB = 1 MB
example.txt
total time: 864.13 ms = 0.86413 s
total time: min:sec.ms: 00:00.864
file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB
speed: 115723329.823 B/s = 113011.064 KB/s = 110.362 MB/s
