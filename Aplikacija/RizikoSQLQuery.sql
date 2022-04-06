--Mapa
INSERT INTO dbo.Map (MapName) VALUES ('World');
--INSERT INTO dbo.Map (MapName) VALUES ('Rome');
--Kontinenti
INSERT INTO dbo.Continent (ContinentName,BonusTanks,MapId) VALUES ('Asia',7,1);
INSERT INTO dbo.Continent (ContinentName,BonusTanks,MapId) VALUES ('Europe',5,1);
INSERT INTO dbo.Continent (ContinentName,BonusTanks,MapId) VALUES ('North America',5,1);
INSERT INTO dbo.Continent (ContinentName,BonusTanks,MapId) VALUES ('South America',2,1);
INSERT INTO dbo.Continent (ContinentName,BonusTanks,MapId) VALUES ('Africa',3,1);
INSERT INTO dbo.Continent (ContinentName,BonusTanks,MapId) VALUES ('Australia',2,1);
--Kontinent Asia
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Afghanistan',1);--1
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('China',1);--2
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Irkutsk',1);--3
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Japan',1);--4
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Kamchatka',1);--5
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Middle East',1);--6
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Mongolia',1);--7
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Siam',1);--8
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Siberia',1);--9
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Ural',1);--10
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Yakutsk',1);--11
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('India',1);--12
--Kontinent Europe
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Iceland',2);--13
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Great Britain',2);--14
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Eastern Europe',2);--15
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Central Europe',2);--16
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Scandinavia',2);--17
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Southern Europe',2);--18
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Western Europe',2);--19
--Kontinent North America
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Alaska',3);--20
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Alberta',3);--21
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Central America',3);--22
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Eastern US',3);--23
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Northwest Territory',3);--24
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Ontario',3);--25
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Quebec',3);--26
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Greenland',3);--27
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Western US',3);--28
-- Kontinent South America
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Venezuela',4);--29
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Argentina',4);--30
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Brazil',4);--31
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Peru',4);--32
--Kontinent Africa
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Congo',5);--33
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('East Africa',5);--34
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Egypt',5);--35
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Madagascar',5);--36
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('North Africa',5);--37
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('South Africa',5);--38
--Kontinent Australia
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Eastern Australia',6);--39
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Indonesia',6);--40
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('New Guinea',6);--41
INSERT INTO dbo.Province (ProvinceName,ContinentId) VALUES ('Western Australia',6);--42

--ProvinceProvince

--Asia

--Afghanistan(India,Middle East,China,Ural,Easter Europe)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (1,6);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (1,12);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (1,2);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (1,10);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (1,15);
--China(Mogolia,India,Ural,Siam,Siberia,Afghanistan)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (2,7);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (2,12);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (2,10);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (2,8);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (2,9);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (2,1);
--India(Siam,Middle East,China,Afghanistan)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (12,8);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (12,6);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (12,1);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (12,2);
--Middle East(Eastern Europe,Southern Europe,India,Afghanistan,Egypt)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (6,15);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (6,18);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (6,12);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (6,35);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (6,1);
--Siam(India,China,Indonesia,New Guinea)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (8,12);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (8,2);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (8,40);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (8,41);
--Ural(Siberia,Eastern Europe,Afghanistan,China)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (10,2);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (10,1);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (10,15);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (10,9);
--Siberia(Ural,China,Mongolia,Irkutsk,Yakutsk)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (9,10);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (9,2);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (9,7);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (9,11);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (9,3);
--Mongolia(China,Japan,Kamthcatka,Irkutsk,Siberia)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (7,2);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (7,3);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (7,4);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (7,9);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (7,5);
--Japan(Mongolia, Kamtchatka)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (4,7);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (4,5);
--Kamtchatka(Japan,Mongolia,Irkutsk,Yakutsk,Alaska)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (5,4);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (5,3);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (5,11);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (5,7);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (5,20);
--Irkutsk(Siberia,Mongolia,Yakutsk,Kamtchatka)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (3,5);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (3,9);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (3,11);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (3,4);
--Yakutsk(Siberia,Irkutsk,Kamtchatka)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (11,3);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (11,5);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (11,9);

--Europe

--Eastearn Europe(Ural,Afghanistan,Middle East,Southern Europe,Central Europe,Scandinavia)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (15,10);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (15,1);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (15,6);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (15,18);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (15,16);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (15,17);
--Scandinavia(Eastern Europe,Central Europe,Great Britain, Iceland)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (17,15);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (17,16);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (17,14);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (17,13);
--Central Europe(Eastern Europe,Scandinavia,Southern Europe,Western Europe,Great Britain)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (16,15);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (16,17);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (16,18);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (16,19);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (16,14);
--Southern Europe(Eastern Europe,Central Europe,Western Europe,North Africa, Egypt)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (18,15);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (18,16);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (18,19);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (18,35);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (18,37);
--Western Europe(Southern Europe,Central Europe,North Africa,Great Britain)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (19,18);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (19,16);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (19,37);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (19,14);
--Great Britain
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (14,13);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (14,19);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (14,18);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (14,17);
--Iceland
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (13,14);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (13,17);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (13,27);

--North America

--Greenland(Iceland,Ontario,Quebec,Northwest territory)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (27,13);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (27,25);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (27,26);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (27,24);
--Northwest territory(Greenland,Alaska,Alberta,Ontario)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (24,27);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (24,20);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (24,21);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (24,25);
--Alaska(Kamtchatka,Northwest,Alberta)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (20,5);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (20,24);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (20,21);
--Alberta(Alaska,Northwest,Ontario,Western)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (21,20);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (21,24);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (21,25);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (21,28);
--Ontario(Greenlan,Quebec,Northwest,Alberta,Western,Eastern)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (25,27);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (25,26);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (25,24);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (25,21);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (25,28);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (25,23);
--Quebec(Greenland,Ontario,Eastern)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (26,27);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (26,25);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (26,23);
--Western(Alberta,Ontario,Eastern,Central)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (28,21);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (28,25);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (28,23);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (28,22);
--Eastern(Quebec,Ontario,Western,Central)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (23,25);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (23,26);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (23,28);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (23,22);
--Central(Western,Eastern,Venezuela)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (22,28);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (22,23);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (22,29);

--South America

--Venezuela(Central,Peru,Brazil)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (29,22);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (29,32);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (29,31);
--Peru(Venezuela,Argentina,Brazil)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (32,29);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (32,30);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (32,31);
--Argentina(Brazil,Peru)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (30,31);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (30,32);
--Brazil(Venezuela,Peru,Argentina,North Africa)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (31,29);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (31,30);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (31,32);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (31,37);

--Africa

--North Africa(Brazil,Egypt,Congo,EastAfrica,Western Europe, Southern Europe)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (37,31);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (37,18);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (37,19);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (37,34);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (37,33);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (37,35);
--Egypt(Southern Europe,Middle east,North Africa,East Africa)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (35,18);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (35,6);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (35,37);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (35,34);
--East Africa(North africa,Egypt,Madagascar,Congo,South)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (34,37);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (34,35);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (34,36);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (34,33);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (34,38);
--Congo(North,East,South)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (33,37);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (33,34);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (33,38);
--South Africa(Congo,East,Madagascar)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (38,33);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (38,34);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (38,36);
--Madagascar(South,East)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (36,34);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (36,38);

--Australia

--Indonesia(Siam,New Guinea,Western)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (40,8);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (40,41);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (40,42);
--New Guinea(Indonesia, Eastern)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (41,40);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (41,39);
--Eastern Australia(New Guinea, Western)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (39,41);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (39,42);
--Western Australia(Indonesia, Eastern)
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (42,40);
INSERT INTO dbo.ProvinceProvince(ConnectedId,ConnectedToId) VALUES (42,39);












