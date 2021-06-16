# global variable
set $infr=0
set $insr=0
set $insw=0

# global func
define printt
	print jiffies
end

# ---------------printscreen-----------------
## 6
break tty_io.c:326
  comm
    start_output
		  printt
      print current->pid
      bt 8
      printscreen
    stop_output
  end

source syscall.gdb
source process.gdb
source file.gdb
