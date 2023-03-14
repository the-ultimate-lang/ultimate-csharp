QT += core gui widgets
equals(QT_MAJOR_VERSION, 6):QT += core5compat

CONFIG += c++17
CONFIG += console
CONFIG += force_debug_info

#TEMPLATE = lib
#CONFIG += staticlib
#CONFIG += dll

#DEFINES += QT_DISABLE_DEPRECATED_BEFORE=0x060000
DEFINES += DEBUG_LINE

gcc:QMAKE_CXXFLAGS_WARN_ON += -Wno-unused-parameter -Wno-unused-function -Wno-cast-function-type
msvc:QMAKE_CXXFLAGS += /bigobj

INCLUDEPATH += $$PWD

LIBS += -L$$[QT_INSTALL_PREFIX]/lib

DESTDIR = $$PWD

TARGET = $${TARGET}-$${QMAKE_HOST.arch}
#message($$QMAKE_QMAKE)
contains(QMAKE_QMAKE, .*static.*) {
    message( "[STATIC BUILD]" )
    DEFINES += QT_STATIC_BUILD
    TARGET = $${TARGET}-static
} else {
    message( "[SHARED BUILD]" )
}

include($$(HOME)/common/include/include.pri)
include($$(HOME)/common/boost/boost.pri)
include($$(HOME)/common/common/common.pri)
include($$(HOME)/common/common2/common2.pri)

include($$PWD/../lib/lib.pri)

HEADERS += utf8LogHandler.h

SOURCES += \
    exe.cpp
