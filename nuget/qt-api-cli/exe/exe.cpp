#include <QtCore>
#include <QtGui>
#include <QtWidgets>
#include "utf8LogHandler.h"
#include "debug_line.h"

#include "mylib.h"
#include "namedpipe.h"

#if 0x0
class ClientThread : public QThread
{
public:
   //explicit ClientThread();
   void run()
   {
       NamedPipeClient ncli("abc");
       ncli.writeString("hello");
       ncli.writeString("world");
       qDebug() << "ncli finish";
   }
};
#endif

int main(int argc, char *argv[])
{
    QApplication app(argc, argv);
    qInstallMessageHandler(utf8LogHandler);
    qdebug_line1("Hello World!");
    qdebug_line1(MyLib::add2(11, 22));
    QString dt = "2014-10-10T13:50:40+0900";
    //qDebug() << QDateTime::fromString(dt, "yyyy-MM-ddThh:mm:ss.zzzZ");
    //qDebug() << QDateTime::fromString(dt, "yyyy-MM-ddThh:mm:ssZ");
    QVariant v = dt;
    qDebug() << v.toDateTime();
    v = "2014-10-10T13:50:40.12356+0900";
    qDebug() << v.toDateTime();
    QString pipeName = "xyz";
    class svr_ : public QThread
    {
        QString pipeName_;
    public:
        svr_(QString pipeName)
            : pipeName_(pipeName)
        {
        }
        void run()
        {
            NamedPipeServer nsvr(pipeName_);
            qDebug() << nsvr.readString();
            qDebug() << nsvr.readString();
            qDebug() << "nsvr finish";
        }
    } svr(pipeName);
    svr.start();
    class cli_ : public QThread
    {
        QString pipeName_;
    public:
        cli_(QString pipeName)
            : pipeName_(pipeName)
        {
        }
        void run()
        {
            NamedPipeClient ncli(pipeName_);
            ncli.writeString("hello");
            ncli.writeString("world");
            qDebug() << "ncli finish";
        }
    } cli(pipeName);
    cli.start();
    //while(svr.isRunning()) QThread::msleep(10);
    //while(cli.isRunning()) QThread::msleep(10);
    cli.wait();
    svr.wait();
    ////QThread::sleep(1000);
    return 0; // return app.exec();
}
