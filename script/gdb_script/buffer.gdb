# ------------char_dev.c--------------
# =============rw_char()==============
# check inode type
## 38
break char_dev.c:103 if $insw>=1 && (current->pid==9||current->pid==10||current->pid==11)
  comm
    start_output
      printt
      print current->pid
      print MAJOR(dev)
      set $insw = $insw - 1
    stop_output
  end
# -----------file_dev.c--------------
# ==========file_read()===============
# step into
## 39
break file_dev.c:22 if $insr>=1 && (current->pid==9||current->pid==10||current->pid==11)
  comm
    start_output
      set $infr=$infr + 1
    stop_output
  end
# look for block number
## 40
break file_dev.c:30 if $insr>=1 && (current->pid==9||current->pid==10||current->pid==11)
  comm
    start_output
      printt
      print current->pid
      print nr
      print inode->i_dev
    stop_output
  end
# change left and char size
## 41
break file_dev.c:34 if $insr>=1 && (current->pid==9||current->pid==10||current->pid==11)
  comm
    start_output
      printt
      print current->pid
      print nr
      print chars
      print filp->f_pos
      print left
    stop_output
  end
## copy to buffer
## 42
break file_dev.c:36 if $insr>=1 && (current->pid==9||current->pid==10||current->pid==11)
  comm
    start_output
      printt
      print current->pid
      print p
    stop_output
  end
# step out
## 43
break file_dev.c:45 if $insr>=1 && (current->pid==9||current->pid==10||current->pid==11)
  comm
    start_output
      set $infr=$infr - 1
      set $insr=$insr - 1
    stop_output
  end
# --------------buffer.c----------------
## =============bread()=================
# before updating
##  if (bh->b_uptodate)
## 44
break buffer.c:278 if $infr>=1 && $insr>=1 && (current->pid==9||current->pid==10||current->pid==11)
  comm
    start_output
      printt
      print current->pid
      bt
      print bh->b_uptodate
    stop_output
  end

# after updating
# 	if (bh->b_uptodate)
## 45
break buffer.c:280 if $infr>=1 && $insr>=1 && (current->pid==9||current->pid==10||current->pid==11)
  comm
    start_output
      printt
      print current->pid
      print bh->b_uptodate
    stop_output
  end

# locked the block
# =========wait_on_buffer()=============
## 46
break buffer.c:40 if $infr>=1 && $insr>=1 && (current->pid==9||current->pid==10||current->pid==11)
  comm
    start_output
      printt
      print current->pid
      bt
      print bh->b_lock
    stop_output
  end
# locked
## 47
break buffer.c:43 if $infr>=1 && $insr>=1 && (current->pid==9||current->pid==10||current->pid==11)
  comm
    start_output
      printt
      print current->pid
      bt
    stop_output
  end
