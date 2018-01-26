import GraphEngine as ge

print("hello")

c = ge.NewCell("C1")
c.RemoveField("baz")
print(c.HasField("baz"))
d = c.GetField("foo")
print(d)
# print(len(d))
# print(d.__class__, d.__dir__())
print("world")
del c
print("end")