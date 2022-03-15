# multithreaded-gzip

## Description

Software processes (compress or decompress) file using gzip and writes it to result file. Processing are done by parts concyrrently.

You can get example.txt file here  
	https://stackoverflow.com/questions/44492576/looking-for-large-text-files-for-testing-compression-in-all-sizes  
	http://mattmahoney.net/dc/textdata.html  

I used an enwik8 from http://mattmahoney.net/dc/textdata.html  
  
**example.txt**  
**file size: 100000001 B = 97656.251 KB = 95.367 MB = 0.093 GB**  

# Stream compression. Single thread.

buffer size | total time (min:sec.ms) | speed
------------|-------------------------|------
8 KB | 00:03.366 | 29007.42 KB/s = 28.327 MB/s
16 KB | 00:03.293 | 29651.738 KB/s = 28.957 MB/s
32 KB | 00:03.128 | 31212.709 KB/s = 30.481 MB/s
64 KB | 00:03.150 | 30992.807 KB/s = 30.266 MB/s
128 KB | 00:03.139 | 31101.474 KB/s = 30.372 MB/s
256 KB | 00:03.111 | 31390.608 KB/s = 30.655 MB/s
512 KB | 00:03.109 | 31409.44 KB/s = 30.672 MB/s
1024 KB | 00:03.163 | 30869.84 KB/s = 30.146 MB/s 


# Stream compression. Multiple threads via pLinq.

buffer size | total time (min:sec.ms) | speed
------------|-------------------------|------
8 KB | 00:00.948 | 102935.873 KB/s = 100.523 MB/s
16 KB | 00:00.749 | 130274.427 KB/s = 127.221 MB/s
32 KB | 00:00.795 | 122833.957 KB/s = 119.954 MB/s
64 KB | 00:00.839 | 116392.804 KB/s = 113.664 MB/s
128 KB | 00:00.866 | 112636.981 KB/s = 109.997 MB/s
256 KB | 00:00.832 | 117303.355 KB/s = 114.554 MB/s
512 KB | 00:00.864 | 112972.876 KB/s = 110.325 MB/s
1024 KB | 00:00.846 | 115411.326 KB/s = 112.706 MB/s


# Stream compression. Multiple threads custom implementation via thread pool with tasks.

buffer size | total time (min:sec.ms) | speed
------------|-------------------------|------
8 KB | 00:01.149 | 84968.736 KB/s = 82.977 MB/s
16 KB | 00:00.822 | 118685.997 KB/s = 115.904 MB/s
32 KB | 00:00.821 | 118892.018 KB/s = 116.105 MB/s
64 KB | 00:00.849 | 114944.448 KB/s = 112.25 MB/s
128 KB | 00:00.901 | 108295.481 KB/s = 105.757 MB/s
256 KB | 00:00.937 | 104128.26 KB/s = 101.687 MB/s
512 KB | 00:00.971 | 100535.031 KB/s = 98.178 MB/s
1024 KB | 00:00.958 | 101931.237 KB/s = 99.542 MB/s


# Stream compression. Multiple threads custom implementation via static threads.

buffer size | total time (min:sec.ms) | speed
------------|-------------------------|------
8 KB | 00:00.899 | 108540.931 KB/s = 105.997 MB/s
16 KB | 00:00.767 | 127165.783 KB/s = 124.185 MB/s
32 KB | 00:00.772 | 126351.89 KB/s = 123.39 MB/s
64 KB | 00:00.863 | 113117.908 KB/s = 110.466 MB/s
128 KB | 00:00.842 | 115852.991 KB/s = 113.137 MB/s
256 KB | 00:00.866 | 112736.519 KB/s = 110.094 MB/s
512 KB | 00:00.856 | 114018.272 KB/s = 111.345 MB/s
1024 KB | 00:00.864 | 113011.064 KB/s = 110.362 MB/s
