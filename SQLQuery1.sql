SELECT a.Id, Model, Price, b.Name, c.Name FROM Cars as a
INNER JOIN Colors as b ON
a.Color = b.Id
INNER JOIN Manufacturers as c ON
a.Manufacturer = c.Id