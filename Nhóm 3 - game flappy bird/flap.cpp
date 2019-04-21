#include <stdio.h>
#include <string.h>
#include <windows.h>
#include <stdlib.h>
#include <conio.h>
#include "console.h"
#include <time.h>
int m=42,n=28,h,k=7,diem=0,out=0,diemcao=0;
struct toado{
	int x,y;
};
struct moto{
	char kytu;
	char mau;
};
struct moto buffer[20][30];
enum trangthai{len,xuong};
struct chimm{
	toado dot[4];
	trangthai tt;
};
void khoitao( chimm &c){
	int i,j;
	m=42;
	n=28;
	c.tt=xuong;
	for(i=0;i<4;i++){
		c.dot[i].x=i+3;
		c.dot[i].y=10;
	}
	for(i=0;i<20;i++)
		for(j=0;j<30;j++){
			//TextColor(10); 
			buffer[j][i].mau=13;
			buffer[i][j].kytu=' ';
		}
}
void hienthi(chimm &c){
	int i,j;	
	buffer[c.dot[0].x][c.dot[0].y].kytu='(';
	buffer[c.dot[1].x][c.dot[1].y].kytu='-';
	buffer[c.dot[2].x][c.dot[2].y].kytu=')';
	buffer[c.dot[3].x][c.dot[3].y].kytu='>';
	buffer[c.dot[0].x][c.dot[0].y].mau=12;
	buffer[c.dot[1].x][c.dot[1].y].mau=12;
	buffer[c.dot[2].x][c.dot[2].y].mau=12;
	buffer[c.dot[3].x][c.dot[3].y].mau=12;	
	for(i=0;i<20;i++){
		buffer[0][i].kytu=219;
		buffer[29][i].kytu=219;
	}
	for(i=0;i<30;i++){
		buffer[i][0].kytu=219;
		buffer[i][19].kytu=219;
	}
	if(m>=28) {
		h= 2+rand() %8;
		m--;
	}
	else if(m > 0) {
	for(j=1;j<h;j++){
		buffer[m][j].kytu=buffer[m+1][j].kytu=196;
		buffer[m][j].mau=buffer[m+1][j].mau=10;
	}
	for(j=h+5;j<19;j++){
		buffer[m][j].kytu=buffer[m+1][j].kytu=196;
		buffer[m][j].mau=buffer[m+1][j].mau=10;
	}
	m--;
	}
	else 
		m=28;
	if(n==28) {
		k= 2+rand() %8;
		n--;
	}
	else if(n > 0) {
	for(j=1;j<k;j++){
		buffer[n][j].kytu=buffer[n+1][j].kytu=196;
		buffer[n][j].mau=buffer[n+1][j].mau=10;
	}
	for(j=k+5;j<19;j++){
		buffer[n][j].kytu=buffer[n+1][j].kytu=196;
		buffer[n][j].mau=buffer[n+1][j].mau=10;
	}
	n--;
	}
	else 
		n=28;
	gotoXY(0,0);
	for(i=0;i<20;i++){
		for(j=0;j<30;j++){
			TextColor(buffer[j][i].mau);
			putchar(buffer[j][i].kytu);
			buffer[j][i].kytu=' ';
		}
		if(i<19) printf("\n");
	}
	if(m==1 || n==1) diem++;
	gotoXY(35,7);
	TextColor(12);
	printf("DIEM: %d",diem);
	gotoXY(35,10);
	printf("DIEM CAO: %d",diemcao);
}
void dieukhien(chimm &c){
	int i;
	if(kbhit()){
		char key= getch();
		if(key == 'w' || key == 'W') c.tt=len;
	}
	if(c.tt == len){
		for(i=0;i<4;i++)
			buffer[c.dot[i].x][c.dot[i].y].kytu=' ';
		for(i=0;i<4;i++)
		c.dot[i].y-=3;
	}
	c.tt=xuong;
	for(i=0;i<4;i++)
	buffer[c.dot[i].x][c.dot[i].y].kytu=' ';
	//c.y++;
	for(i=0;i<4;i++)
		c.dot[i].y++;
	Sleep(150);
}
void Nocursortype() // ham an con tro chuot man hinh console
{
	CONSOLE_CURSOR_INFO Info;
	Info.bVisible = FALSE;
	Info.dwSize = 20;
	SetConsoleCursorInfo(GetStdHandle(STD_OUTPUT_HANDLE), &Info);
}
int main(){
	SetConsoleTitle("Game Flap");
	chimm c;
	fflush(stdin);
	srand(time(NULL));
	Nocursortype();
	fflush(stdin);
	while(1){
		khoitao(c);
	TextColor(14);
	gotoXY(45,10);
	printf("BAN DA SAN SANG!!!\n");
	gotoXY(45,15);
	printf("BAM 1 PHIM BAT KY DE BAT DAU\n");
	gotoXY(45,20);
	printf("BAM PHIM W DE DI CHUYEN");
	while(1){
		if(kbhit()){
		char key= getch();
		if(key != '\0') break;
		}
	}
	clrscr();
	if(diem > diemcao)
		diemcao=diem;
	diem=0;
	while(1){
		hienthi(c);
		if((m > 1 && m<7) && ((c.dot[0].y < h) || (c.dot[0].y >(h+4))) ){
			gotoXY(35,12);
			printf("GAME OVER!!!\n");
			break;
		}
		else if((n>1 && n<7) && ((c.dot[0].y < k) || (c.dot[0].y >(k+4))) ){
			gotoXY(35,12);
			printf("GAME OVER!!!\n");
			break;
		}
		else if(c.dot[0].y == 0 || c.dot[0].y == 19){
			gotoXY(35,13);
			printf("GAME OVER!!!\n");
			break;
		}
		dieukhien(c);
	}
	gotoXY(35,16);
	printf("BAM PHIM SO 1 DE CHOI LAI!!!");
	gotoXY(35,19);
	printf("BAM PHIM SO O DE THOAT GAME!!!");
	while(1){
		if(kbhit()){
		char key= getch();
		if(key == 49) break;
		else if(key == 48){
			out=1;
			break;
		}
		}
	}
	if(out == 1) break;
	clrscr();
	}
}
