# -----------------ll_rw_blk.c-----------
# ============lock_buffer()==============
# sleep while locked by others
## 48
## none
break ll_rw_blk.c:46 if $infr>=1&&$insr>=1 && (current->pid==9||current->pid==10||current->pid==11)
  comm
    start_output
      printt
      print current->pid
    stop_output
  end
# lock the buffer
# 49
break ll_rw_blk.c:48 if $infr>=1&&$insr>=1 && (current->pid==9||current->pid==10||current->pid==11)
  comm
    start_output
      printt
      print current->pid
    stop_output
  end
# ============add_request()=============
# no former request
## 50
break ll_rw_blk.c:75 if $infr>=1&&$insr>=1 && (current->pid==9||current->pid==10||current->pid==11)
  comm
    start_output
      printt
      print current->pid
    stop_output
  end
# existing request 
## 51
break ll_rw_blk.c:85 if $infr>=1&&$insr>=1 && (current->pid==9||current->pid==10||current->pid==11)
  comm
    start_output
      printt
      print current->pid
      print tmp
      print tmp->next
    stop_output
  end
# ============make_request()=============
# send request
# 	add_request(major+blk_dev,req);
## 52
break ll_rw_blk.c:142 if $infr>=1&&$insr>=1 && (current->pid==9||current->pid==10||current->pid==11)
  comm
    start_output
      printt
      print current->pid
      print req->dev
	  print req->cmd
	  print req->errors
	  print req->sector 
	  print req->nr_sectors 
	  print req->buffer 
	  print req->waiting 
	  print req->bh
	  print req->next
    stop_output
  end

# ---------------floppy.c----------------
# ==========rw_interrupt()===============
## 53
#*****************************************
# interrupt and wake up, then request next
#****************************************
break floppy.c:265 if $infr>=1 && $insr>=1
  comm
    start_output
      printt
      print current->pid
      bt
    stop_output
   end
# ==========do_fd_request()==========
#	add_timer(ticks_to_floppy_on(curr
## ent_drive),&floppy_on_interrupt);
## 54
break floppy.c:455 if $infr>=1&&$insr>=1
  comm
    start_output
      printt
      print current->pid
      bt
    stop_output
  end

# -----------------blk.h------------------
# ===========unlock_buffer()==============
##	wake_up(&bh->b_wait);
## 55
#*****************************************
# bh->b_lock=0
#****************************************
break blk.h:106 if $infr>=1&&$insr>=1
  comm
    start_output
      printt
      print current->pid
      print &bh->b_wait
    stop_output
  end
# ============end_request()===============
##	wake_up(&CURRENT->waiting);
## 56
break blk.h:121 if $infr>=1&&$insr>=1
  comm
    start_output
      printt
      print current->pid
      print &CURRENT->waiting
    stop_output
  end
##	wake_up(&wait_for_request);
## 57
break blk.h:122 if $infr>=1&&$insr>=1
  comm
    start_output
      printt
      print current->pid
      print &wait_for_request
    stop_output
  end
##	wake_up(&wait_for_request);
## 58
break blk.h:124 if $infr>=1&&$insr>=1
  comm
    start_output
      printt
      print current->pid
      print CURRENT->dev
      print CURRENT->next
    stop_output
  end