TEMPLATE = subdirs

SUBDIRS += exe
SUBDIRS += lib
exe.depends += lib

#SUBDIRS += $$(HOME)/common/common
#lib.depends += $$(HOME)/common/common
#exe.depends += $$(HOME)/common/common

SUBDIRS += $$(HOME)/common/common2
lib.depends += $$(HOME)/common/common2
exe.depends += $$(HOME)/common/common2
