# ---------------systemcall.c---------------
# ===============system_call=================
# fork execve write read open waitpid  
## 7
break system_call.s:94 if $eax==2||$eax==3||$eax==4||$eax==5||$eax==7||$eax==11
  comm
    start_output
	    printt
      print $eax
      x/12x $sp
    stop_output
  end
# ================sys_execve================
## 8
break system_call.s:203 
  comm
    start_output
	    printt
      x/x $eax
      x/2x $sp 
      # eax eip
    stop_output
  end
# ==================sys_fork=================
# push, return from find empty process
## 9
break system_call.s:210
  comm
    start_output
	    printt
      x/x $eax
      x/13x $sp 
      # eip
    stop_output
  end

# before calling copy process
## 10
break system_call.s:217
  comm
    start_output
	    printt
      x/18x $sp 
      # add five
    stop_output
  end

# remove 5 on the stack
## 11
break system_call.s:218
  comm
    start_output
	    printt
    stop_output
  end
# ============floppy_interrupt=================
# push and set eax to do_floppy
# we dont know more about the original stack/
# before the kernal state
## 12
break system_call.s:259
  comm
    start_output
	  printt
    x/x do_floppy
    x/6x $sp
    stop_output
  end
# pop
## 13
break system_call.s:272
  comm
    start_output
	    printt
    stop_output
  end
#-------------------exec.c------------------
# ================do_execve()================
# step into
## 14
break exec.c:221
  comm
    start_output
	    printt
      print current->pid
      set $infr=1
      set $insr=1
    stop_output
  end
# exit
## 15
break exec.c:348
  comm
    start_output
	    printt
      print current->pid
      x/x eip[0]
      x/x eip[3]
      x/5x $eip
      set $infr=0
      set $insr=0
    stop_output
  end
# ------------------fork.c-----------------
# =============find_empty_process()=========
#		if (!task[i])
# 		return i
## check if last pid dulplicates and 
## find new empty process
## 16
break fork.c:149
  comm
    start_output
      printt
      print current->pid
      bt 8
	    print i
      print task[i]?task[i]->pid:-1
      print last_pid
    stop_output
  end

