#include<stdio.h>
#include<windows.h>
#include<conio.h>
#include<stdlib.h>
#include<time.h>
#include "console.h"

#define trai 0
#define phai 60
#define tren 0
#define duoi 20


struct ToaDo{
	int x,y;
};

struct MotO{
	char kitu;
	char mau;
};

MotO buffer[duoi - tren][phai - trai+1];

void VeMotO(int dong, int cot, char kitu, int mau){
	buffer[dong][cot].kitu = kitu;
	buffer[dong][cot].mau = mau;
}


enum TrangThai{UP, DOWN, LEFT, RIGHT};

struct Snake{
	ToaDo dot[50];
	int n;
	TrangThai tt;
};
struct Hoaqua{
	ToaDo td;
};
void diemto(Hoaqua &hqto){
	hqto.td.x = trai +1 + rand() % (phai - trai -3);
	hqto.td.y = tren +1 + rand() % (duoi - tren -3);
}

void KhoiTao(Snake &snake, Hoaqua  &hq, Hoaqua	&hqto){
	// khoi tao ran
	snake.dot[0].x= trai + 1 ;
	snake.dot[0].y = tren + 1;
	snake.n = 1;
	snake.tt= RIGHT;
	
	//khoi tao hq
	hq.td.x = trai +2 + rand() % (phai - trai -3);
	hq.td.y = tren +2 + rand() % (duoi - tren -3 );
	
	//khoi tao hqto
		hqto.td.x = trai +2 + rand() % (phai - trai -3);
		hqto.td.y = tren +2 + rand() % (duoi - tren -3 );
	
	// khoi tao buffer
	for(int i = trai ; i < phai; i++)
		for(int j = tren; j < duoi; j++)
			VeMotO(j,i,' ', 0);
	
}

void HienThi(Snake snake, Hoaqua hq,Hoaqua	hqto, int diem){
	//clrscr();
	int i;
	// hien thi tuong
	for(i = tren ; i <= duoi; i++){
		VeMotO(i,trai, 179, 10);
		VeMotO(i,phai -1, 179, 10);
	}

	for(i = trai ; i <= phai; i++){
	
		VeMotO(tren,i, 196, 10);
		VeMotO(duoi - 1, i, 196, 10);
	}

	VeMotO(hq.td.y, hq.td.x, 3, 11);
	// ve ran
	
	VeMotO(snake.dot[0].y, snake.dot[0].x, 2, 12);
	for(i = 1; i < snake.n; i++){
	
		VeMotO(snake.dot[i].y, snake.dot[i].x, 42, 14);
	}
	
	if(snake.n %3== 1){
		VeMotO(hqto.td.y, hqto.td.x, 15, 13);
	}
	//tu buffer in ra man hinh
	gotoXY(trai,tren);
	for(i = tren; i < duoi ; i++)
	{
		for(int j = trai ; j < phai; j++){
			TextColor(buffer[i][j].mau);
			putchar(buffer[i][j].kitu);
			buffer[i][j].kitu = ' ';
		}
		printf("\n");
	}
	gotoXY(10, duoi + 1);
	printf("Diem = %d", diem);
}

void  DieuKhien(Snake &snake){
	// truyen dia chi cho dot 2 -> n-1
	for(int i = snake.n-1; i > 0; i--)
		snake.dot[i] = snake.dot[i-1];
	
	//dieu khien
	if(kbhit()){
		char key = getch();
		if(key == 'A' || key == 'a'){
			if(snake.tt == RIGHT)
				snake.tt = RIGHT;
			else
			snake.tt = LEFT;
		}
			
		else if(key == 'S' || key == 's'){
			if(snake.tt == UP)
				snake.tt = UP;
			else
			snake.tt = DOWN;
		}
		else if(key == 'D' || key == 'd'){
			if(snake.tt == LEFT)
				snake.tt = LEFT;
			else
			snake.tt = RIGHT;
		}
		else if(key == 'W' || key == 'w'){
			if(snake.tt == DOWN)
				snake.tt = DOWN;
			else
			snake.tt = UP;
		}
	
	}
	
	// xu li dieu khien
	
	if(snake.tt == UP)
		snake.dot[0].y--;
	else if(snake.tt == DOWN)
		snake.dot[0].y++;
	else if(snake.tt == LEFT)
		snake.dot[0].x--;
	else
		snake.dot[0].x++;
		
}

int  Xuli( Snake &snake , Hoaqua &hq, Hoaqua &hqto, int &diem, int &timeSleep ){
	// an duoc hoa qua
	if(snake.dot[0].x == hq.td.x && snake.dot[0].y == hq.td.y ){
		
		putchar('\a');
		for(int i = snake.n; i > 0; i--)
			snake.dot[i] = snake.dot[i-1];
		snake.n++;
		diem++;
		if(snake.tt == UP)
			snake.dot[0].y--;
		else if(snake.tt == DOWN)
			snake.dot[0].y++;
		else if(snake.tt == RIGHT )
			snake.dot[0].x++;
		else
			snake.dot[0].x--;
		
		hq.td.x = trai +2 + rand() % (phai - trai -3);
		hq.td.y = tren +2 + rand() % (duoi - tren -3);
	
	}
	// an diem to
	if(snake.dot[0].x == hqto.td.x && snake.dot[0].y == hqto.td.y ){
		putchar('\a');
		for(int i = snake.n; i > 0; i--)
			snake.dot[i] = snake.dot[i-1];
			
		snake.n++;
		diem +=3;
		if(snake.tt == UP)
			snake.dot[0].y --;
		else if(snake.tt == DOWN)
			snake.dot[0].y++;
		else if(snake.tt == RIGHT )
			snake.dot[0].x++;
		else
			snake.dot[0].x--;
			
	if(snake.n %3== 1){
		diemto(hqto);
		timeSleep -= 5;
	}
		
		
	}
	
	if( snake.dot[0].x < trai || snake.dot[0].x > phai-1||
		snake.dot[0].y < tren || snake.dot[0].y > duoi-1)
		return -1;
	for(int i = 1; i < snake.n; i++)
		if(snake.dot[0].x == snake.dot[i].x && 
  		    snake.dot[0].y == snake.dot[i].y)
  		    return -1;
  
}


int main(){
	srand(time(NULL));// khoi tao bo sinh so ngau nhien
	Snake snake;
	Hoaqua hq,hqto;
	int ma;
	int timeSleep = 50;
	int diem = 0;
	//KhoiTao(snake, hq);
	KhoiTao(snake, hq,hqto);
	while(1){
		//Hien thi
		HienThi(snake, hq, hqto, diem);
		
		// Dieu khien
		DieuKhien(snake);
		
		// Xu li
		 ma = Xuli(snake, hq,hqto,diem,timeSleep);
		
		// Thang, thua game
		if(ma == -1){
			gotoXY(10, duoi + 1);
			printf("Thua roi %c %d diem ",2,diem); 
			while(_getch() != 13);
			break;
		}

			
		
		Sleep(timeSleep);
		
	}
}