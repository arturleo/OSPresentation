# -----------------exit.c------------------
# =================release()===============
## release zombie child process
## task[1]=NULL
## 24
break exit.c:28
  comm
    start_output
      printt
      print current->pid
      print i
      print (*p)->pid
    stop_output
  end
# =============sys_waitpid()==============
## exited child process
## 25
## none
break exit.c:169
  comm
    start_output
      printt
      print current->pid
      print (*p)->pid
    stop_output
  end

## zombie child process, release it 
## 26
break exit.c:177
  comm
    start_output
      printt
      print current->pid
      print flag
      print code
    stop_output
  end

## child process running, stuck
## 27
break exit.c:179
  comm
    start_output
      printt
      print current->pid
      print (*p)->pid
    stop_output
  end

## 28
## wait for a child process
## recieve signal
## set state and schedule()
#		current->state=TASK_INTERRUPTIBLE;
#		schedule();
break exit.c:186
  comm
    start_output
      printt
      print current->pid
      print current->state
      print current->signal
    stop_output
  end

# ================do_exit()=============
# close file opened by the program 
## 29
# **************************************
# close process file table and opened 
# files one by one
# **************************************
break exit.c:116
  comm
    start_output
      printt
      print current->pid
      print i
    stop_output
  end

# process exit
## 30
#	tell_father(current->father);
#**************************************
# set task_zombie and exit_code here 
#**************************************
break exit.c:131
  comm
    start_output
      printt
      print current->pid
      print code
    stop_output
  end

# =============tell_father()==============
## tell father if it has
# set signal before
## 31
break exit.c:93
  comm
    start_output
      printt
      print current->pid
      print task[i]->pid
      print task[i]->signal
    stop_output
  end

# set signal after
## 32
break exit.c:94
  comm
    start_output
      print task[i]->pid
      print task[i]->signal
    stop_output
  end