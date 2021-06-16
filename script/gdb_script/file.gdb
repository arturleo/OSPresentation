# -----------------open.c----------------
# ===============sys_open()==============
# find file handle
## 33
break open.c:146 if current->pid==9||current->pid==10||current->pid==11
  comm
    start_output
      printt
      print current->pid
      print fd
      print current->filp[fd]?current->filp[fd]->f_mode:-1
      print current->filp[fd]?current->filp[fd]->f_flags:-1
      print current->filp[fd]?current->filp[fd]->f_count:-1
      print current->filp[fd]?current->filp[fd]->f_inode:-1
      print current->filp[fd]?current->filp[fd]->f_pos:-1
    stop_output
  end
# find empty file table
#			if (!f->f_count) break;
## 34
break open.c:153 if current->pid==9||current->pid==10||current->pid==11
  comm
    start_output
      printt
      print current->pid
      print i
      print f->f_count
    stop_output
  end

# copy inode
## inode from open_namei()
## 35
break open.c:185 if current->pid==9||current->pid==10||current->pid==11
  comm
    start_output
      printt
      print current->pid
      print i
      print f->f_mode
      print f->f_flags
      print f->f_count 
      print f->f_inode
      print f->f_pos 
    stop_output
  end
# ---------------read_write.c------------
# =============sys_read()================
# check inode type
## 36
break read_write.c:77 if current->pid==9||current->pid==10||current->pid==11
  comm
    start_output
      printt
      print current->pid
      set $insr=$insr + 1
      print S_ISCHR(inode->i_mode)
      print S_ISBLK(inode->i_mode)
      print S_ISDIR(inode->i_mode)
      print S_ISREG(inode->i_mode)
    stop_output
  end
# ============sys_write()================
# check inode type
## 37
break read_write.c:96 if current->pid==9||current->pid==10||current->pid==11
  comm
    start_output
      printt
      print current->pid
      set $insw=$insw + 1
      print S_ISCHR(inode->i_mode)
      print S_ISBLK(inode->i_mode)
      print S_ISREG(inode->i_mode)
    stop_output
  end

source buffer.gdb
source hw.gdb
