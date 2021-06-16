# ------------------shed.c--------------------
# =================schedule()=================
# refresh running process state
## 	if ((*p)->state == TASK_RUNNING && 
##(*p)->counter > c)
## 17
break sched.c:132
  comm
    start_output
        printt
        print current->pid
        print i
        print (*p)->pid
        print (*p)->state
        print (*p)->counter
        print (*p)->priority
        print (*p)->father
        print (*p)->exit_code
        print (*p)->signal
        print c
    stop_output
  end

# change counter for each process
##	c = (*p)->counter, next = i;
## 18
break sched.c:133
  comm
    start_output
        printt
        print current->pid
        print i
        print (*p)->counter
    stop_output
  end

# reset counter if none
##	(*p)->counter = ((*p)->counter >> 1) +
##	(*p)->priority;
## 19
break sched.c:138
  comm
    start_output
        printt
        print current->pid
        print (*p)->pid
        print ((*p)->counter >> 1) + (*p)->priority
    stop_output
  end

# switch process
## switch_to(next);
## 20
break sched.c:141
  comm
    start_output
	    printt
      print current->pid
	    bt
      print next
      print task[next]->pid
    stop_output
  end
# ===================sleep_on()=================
# before
## 21
break sched.c:159
  comm
    start_output
      printt
      print current->pid
      bt 
      print tmp?tmp->pid:-1
      print current->state
    stop_output
  end

# after scehdule and return
## 22
break sched.c:164
  comm
    start_output
		  printt
      print current->pid
	    print tmp?tmp->pid:-1
    stop_output
  end
# ===================wake_up()==================
# check p and set state running
#		(*p)->state = TASK_RUNNING;
## 23
break sched.c:191
  comm
    start_output
      printt
      print current->pid
      bt
      # queue
	    print p
      # pid
      print p&&*p?(*p)->pid:-1
    stop_output
  end

# ===================do_timer()==================
# set counter
#	if ((--current->counter)>0) return;
## 59
break sched.c:333
  comm
    start_output
      printt
      print current->pid
      print current->counter-1
    stop_output
  end

source exit.gdb
