#include <stdio.h>
#include <unistd.h>
#include <time.h>
#include <stdlib.h>
#include <sys/times.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>
#include <sys/wait.h>

#define HZ	32

void cpuio_bound(int last, int cpu_time, int io_time)
{
	struct tms start_time, current_time;
	clock_t utime, stime;
	int sleep_time;

	while (last > 0)
	{
		/* CPU Burst */
		times(&start_time);
		do
		{
			times(&current_time);
			utime = current_time.tms_utime - start_time.tms_utime;
			stime = current_time.tms_stime - start_time.tms_stime;
		} while ( ( (utime + stime) / HZ )  < cpu_time );
		last -= cpu_time;

		if (last <= 0 )
			break;

		/* IO Burst */
		sleep_time=0;
		while (sleep_time < io_time)
		{
			sleep(1);
			sleep_time++;
		}
		last -= sleep_time;
	}
}

int main(int argc, char * argv[])
{
	char* content=malloc(6+1);
	int pid1,pid2;
	if(!(pid1=fork())){
		int f=open("./file2.txt",O_RDONLY);
		int r=read(f,content,6);
		cpuio_bound(2,2,0);
		write(1,content,6);
	}
	else if(!(pid2=fork())){
		int f=open("./file1.txt",O_RDONLY);
		int r=read(f,content,6);
		cpuio_bound(1,1,0);
		write(1,content,6);
	}
	else{
		wait(NULL);
		wait(NULL);
	}
	return 0;
}
