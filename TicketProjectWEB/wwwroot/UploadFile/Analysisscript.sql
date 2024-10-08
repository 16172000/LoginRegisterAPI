USE [dbmtbp]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_parts_report_AnalysisList]    Script Date: 4/30/2024 5:53:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--EXEC [sp_get_parts_report_AnalysisList] '2018-04-01','2019-03-31','2019-04-01','2020-03-31',NULL,NULL,NULL,NULL,'SQ11,SQ12,SQ13,SQ14,SQ15,SQ16,SQ17,SQ18,SQ21,SQ22,SQ23,SQ24,SQ25,SQ26,SQ27,SQ28','FS01,FS02','02,03,0010,11','30 to 60 days','11'
CREATE PROCEDURE [dbo].[sp_get_parts_report_AnalysisList]
(            
@startDate  NVARCHAR(MAX) ,  
@endDate  NVARCHAR(MAX) ,  
@startDateNew  NVARCHAR(MAX) ,  
@endDateNew  NVARCHAR(MAX) ,
@MultipleSupplierCode  NVARCHAR(MAX) ,
@MultipleParts  NVARCHAR(MAX) ,
@MultipleFinancialYear  NVARCHAR(MAX) ,
@Commodity  NVARCHAR(MAX) ,
@WorkCenter  NVARCHAR(MAX) ,
@Plant NVARCHAR(MAX),
@DescionCode NVARCHAR(MAX),
@header NVARCHAR(MAX),
@Id NVARCHAR(MAX)

)
AS                        
BEGIN
--select getdate()
DECLARE @Multi_Commodity NVARCHAR(MAX);



 
 --UPDATE STATISTICS tbsapdata;



SET @Multi_Commodity= (SELECT STRING_AGG(Commodity, ',') 
FROM (SELECT DISTINCT c.Name as Commodity 
FROM [dbo].[mst_Commodity] c
join mst_CommodityHeader ch on ch.Id=c.CHid
WHERE ch.Is_Active = 1 
AND ( c.WorkCenter IN (SELECT VALUE FROM string_split(@WorkCenter, ','))) 
) as Commo)

 CREATE TABLE #TempTableN (
  ID INT ,
  Reference_Period VARCHAR(20)
);

 CREATE TABLE #TempTableNA (
  ID INT ,
  Reference_Period VARCHAR(20)
);

INSERT INTO #TempTableN
 values(1,'< 7 days') 
 ,(2,'7 to 15 days') 
 ,(3,'15 to 30 days') 
 ,(4,'30 to 60 days') 
 ,(5,'60 to 90 days') 
 ,(6,'90 to 180 days')
 ,(7,'180 to 300 days')
 ,(8,'> 300 days')
 ,(9,'No Defect')
  ,(11,'Total')
 ,(12,'New Supplier Added') 
 --,(11,'No Supply') 

 --,(13,'% Age in Green Zone')

 INSERT INTO #TempTableNA
 values(1,'< 7 days') 
 ,(2,'7 to 15 days') 
 ,(3,'15 to 30 days') 
 ,(4,'30 to 60 days') 
 ,(5,'60 to 90 days') 
 ,(6,'90 to 180 days')
 ,(7,'180 to 300 days')
 ,(8,'> 300 days')
 ,(9,'No Defect')   
 ,(11,'No Supply') 
 ,(12,'Total')
 ,(13,'% Age in Green Zone')

 DECLARE @Totalvendor NVARCHAR(MAX)

SELECT  DISTINCT  Material,Vender,SUM(Inspected_Qty) as Inspected_Qty,
CONCAT(Material,'-',Vender) as MV
into #x from tbsapdata where 
CAST(UD_Date AS DATE) BETWEEN @startDate and @endDate
AND (@MultipleSupplierCode IS NULL OR Vender IN(SELECT VALUE FROM string_split(@MultipleSupplierCode,',')))
AND (@MultipleParts IS NULL OR Material IN(SELECT VALUE FROM string_split(@MultipleParts,',')))
AND (@Plant IS NULL OR Plant IN(SELECT VALUE FROM string_split(@Plant,',')))
AND (@Multi_Commodity IS NULL OR Commodity IN(SELECT distinct VALUE FROM string_split(@Multi_Commodity,',')))
AND (@WorkCenter IS NULL OR WorkCenter IN(SELECT VALUE FROM string_split(@WorkCenter,',')))
group by Vender,Material  -- Total 366 Vendors
OPTION (RECOMPILE);

--select * from #x

DECLARE @TotalvendorCount int;
SET @Totalvendor=(SELECT STRING_AGG(CAST(MV AS VARCHAR(MAX)), ',')  from #x)

SET @TotalvendorCount=(SELECT COunt(Material)  from #x)

---select @Totalvendor, @TotalvendorCount

select DISTINCT Material,Vender,Count(UD_Date)defect,MV,
DATEDIFF(DAY, @startDate , @endDate) as NoOfDays, 
CAST(CAST(DATEDIFF(DAY, @startDate , @endDate) as decimal(18,2))/CAST(COUNT(UD_Date) as decimal(18,2)) as decimal(18,2) ) AS Mtbp into #y 
from(
SELECT  DISTINCT  Material,Vender,CONCAT(Material,'-',Vender) as MV,COUNT(Material) defect,
UD_Date
 from tbsapdata where 
CAST(UD_Date AS DATE) BETWEEN @startDate and @endDate -- Total 280 Vendors defects
 and (LR_Rejected>0 or GR_Rejected>0) 
 AND (@MultipleSupplierCode IS NULL OR Vender IN(SELECT VALUE FROM string_split(@MultipleSupplierCode,',')))
AND (@MultipleParts IS NULL OR Material IN(SELECT VALUE FROM string_split(@MultipleParts,',')))
AND (@Plant IS NULL OR Plant IN(SELECT VALUE FROM string_split(@Plant,',')))
AND (@Multi_Commodity IS NULL OR Commodity IN(SELECT distinct VALUE FROM string_split(@Multi_Commodity,',')))
AND (@WorkCenter IS NULL OR WorkCenter IN(SELECT VALUE FROM string_split(@WorkCenter,',')))
AND (@DescionCode IS NULL OR  Rejection_UD_Code  IN (SELECT VALUE FROM string_split(@DescionCode, ',')) or  RejectionCode IN (SELECT VALUE FROM string_split(@DescionCode, ',')))
 group by Vender,UD_Date,Material)x
 group by Vender,Material,MV

 --select * from #y

 SELECT ROW_NUMBER() over( order  by MTBP_Range) as Id,MTBP_Range,COUNT(Material) as SupplierCount,
 STRING_AGG(cast(Material as nvarchar(MAX)),',') as Vender,
 STRING_AGG(CAST(MV AS VARCHAR(MAX)), ',') AS MV
 into #xNew
 from (
 select #x.Vender,Inspected_Qty,NoOfDays,defect,Mtbp,#x.Material,#x.MV,

 CASE
 WHEN Mtbp<7 THEN '< 7 days'
 WHEN Mtbp>=7 and Mtbp<15 THEN '7 to 15 days' 
 WHEN Mtbp>=15 and mtbp<30 THEN '15 to 30 days'
 WHEN Mtbp>=30 and mtbp<60 THEN '30 to 60 days' 
 WHEN Mtbp>=60 and mtbp<90 THEN '60 to 90 days' 
 WHEN Mtbp>=90 and mtbp<180 THEN '90 to 180 days' 
 WHEN Mtbp>=180 and mtbp<300 THEN '180 to 300 days'
 WHEN Mtbp>=300 THEN '> 300 days' 
 WHEN Mtbp IS NULL THEN 'No Defect'   
ELSE ''
  END 
as MTBP_Range
  
 from #x LEFT join #y on #x.MV = #y.MV--#x.Vender=#y.Vender and #x.Material = #y.Material
 )x group by x.MTBP_Range

 
 
 
 SELECT  #TempTableN.ID,#TempTableN.Reference_Period,SupplierCount,Vender into #xnewReference from(
 SELECT Id,MTBP_Range,SupplierCount,MV as Vender  from #xNew
  UNION
 SELECT 11 as Id,'Total' as MTBP_Range,Sum(SupplierCount) SupplierCount,'' as Vender from #xNew
 UNION
 SELECT 12 as Id,'New Supplier Added' as MTBP_Range,0 SupplierCount,'' as Vender from #xNew
 ) as X RIGHT Join #TempTableN on #TempTableN.Reference_Period collate database_default=x.MTBP_Range collate database_default
 order by #TempTableN.ID asc

-- select getdate()
--select * from #xnewReference
 --SELECT #TempTableN.ID,#TempTableN.Reference_Period,SupplierCount,Vender into #xnewReference 
 --from #TempTableN LEFT Join #XNewMain on #TempTableN.Reference_Period=#XNewMain.MTBP_Range order by #TempTableN.ID asc



 --select * from #xnewReference

 DECLARE @Ven NVARCHAR(MAX);
  
 -----------------------------------------------Analyis period (Duration Wise)-----------------------------------------
 -- 

 --SELECT GETDATE()
SELECT  DISTINCT  Material,Vender,SUM(Inspected_Qty) as Inspected_Qty, 
CONCAT(Material,'-',Vender) as MV
into #xy from tbsapdata where 
CAST(UD_Date AS DATE) BETWEEN @startDateNew and @endDateNew
AND (@MultipleSupplierCode IS NULL OR Vender IN(SELECT VALUE FROM string_split(@MultipleSupplierCode,',')))
AND (@MultipleParts IS NULL OR Material IN(SELECT VALUE FROM string_split(@MultipleParts,',')))
AND (@Plant IS NULL OR Plant IN(SELECT VALUE FROM string_split(@Plant,',')))
AND (@Multi_Commodity IS NULL OR Commodity IN(SELECT distinct VALUE FROM string_split(@Multi_Commodity,',')))
AND (@WorkCenter IS NULL OR WorkCenter IN(SELECT VALUE FROM string_split(@WorkCenter,',')))
group by Vender,Material  -- Total 366 Vendors
OPTION (RECOMPILE);

--SELECT * from #xy
--SELECT GETDATE()

select DISTINCT Material,Vender,Count(UD_Date)defect,MV,
DATEDIFF(DAY, @startDateNew , @endDateNew) as NoOfDays, 
CAST(CAST(DATEDIFF(DAY, @startDateNew , @endDateNew) as decimal(18,2))/CAST(COUNT(UD_Date) as decimal(18,2)) as decimal(18,2) ) AS Mtbp 
into #yx 
from(
SELECT Material,Vender,CONCAT(Material,'-',Vender) as MV,COUNT(UD_Date) defect,
UD_Date
 from tbsapdata where 
CAST(UD_Date AS DATE) BETWEEN @startDateNew and @endDateNew-- Total 280 Vendors defects
 and (LR_Rejected>0 or GR_Rejected>0) 
 AND (@MultipleSupplierCode IS NULL OR Vender IN(SELECT VALUE FROM string_split(@MultipleSupplierCode,',')))
AND (@MultipleParts IS NULL OR Material IN(SELECT VALUE FROM string_split(@MultipleParts,',')))
AND (@Plant IS NULL OR Plant IN(SELECT VALUE FROM string_split(@Plant,',')))
AND (@Multi_Commodity IS NULL OR Commodity IN(SELECT distinct VALUE FROM string_split(@Multi_Commodity,',')))
AND (@WorkCenter IS NULL OR WorkCenter IN(SELECT VALUE FROM string_split(@WorkCenter,',')))
AND (@DescionCode IS NULL OR  Rejection_UD_Code  IN (SELECT VALUE FROM string_split(@DescionCode, ',')) or  RejectionCode IN (SELECT VALUE FROM string_split(@DescionCode, ',')))
 group by Vender,UD_Date,Material
 )x 
 group by Vender,Material,MV

 --select * from #xy
 --select * from #yx

 SELECT ROW_NUMBER() over( order  by MTBP_Range) as Id, MTBP_Range,Vender,Mtbp,NoOfDays,defect,Material,MV 
 into #xyNewA from 
 (SELECT #xy.Vender,defect,NoOfDays,#xy.Material,#xy.MV,
 CASE
 WHEN Mtbp<7 THEN '< 7 days'
 WHEN Mtbp>=7 and Mtbp<15 THEN '7 to 15 days' 
 WHEN Mtbp>=15 and mtbp<30 THEN '15 to 30 days'
 WHEN Mtbp>=30 and mtbp<60 THEN '30 to 60 days' 
 WHEN Mtbp>=60 and mtbp<90 THEN '60 to 90 days' 
 WHEN Mtbp>=90 and mtbp<180 THEN '90 to 180 days' 
 WHEN Mtbp>=180 and mtbp<300 THEN '180 to 300 days'
 WHEN Mtbp>=300 THEN '> 300 days' 
 WHEN Mtbp IS NULL THEN 'No Defect'   
ELSE ''
  END 
as MTBP_Range,Mtbp  from #xy LEFT join #yx on #xy.MV= #yx.MV--#xy.Vender=#yx.Vender and #xy.Material = #yx.Material
)X

--select * froM #xyNewA 
--where Mtbp>=180 and mtbp<300 order by Mtbp desc


--select getdate()
DECLARE @VenderSupplier NVARCHAR(MAX);
SET @VenderSupplier=(SELECT STRING_Agg(CAST(MV AS VARCHAR(MAX)), ',') from #xy 
where MV NOT In(SELECT MV from #x))
--where Material NOT In(SELECT VALUE from String_split(@Totalvendor,','))) ----------New Supplier-------

--select @VenderSupplier


update #xnewReference set Vender=@VenderSupplier where Id=12
update #xnewReference set Vender=@Totalvendor where Id=11
--update #xnewReference set Vender=CONCAT(@Totalvendor,',',@VenderSupplier) where Id=12 ---------TOTAL Supplier Previous and New Supplier Added------------

--select * from #xnewReference
--select @VenderSupplier
--select getdate()

  CREATE TABLE #TempMain(
   ID INT ,
  Reference_Period VARCHAR(20),
  MTBP decimal(18,2),
  Vender nvarchar(max),
  defect int,
  Vender2 nvarchar(max)
);

DECLARE @SupplierCount INT
DECLARE @SupplierCountA INT
DECLARE @COUNT INT
DECLARE @venderCode NVARCHAR(MAX)
    DECLARE @Mtbp INT
    DECLARE @MTBP_Range NVARCHAR(MAX)
    DECLARE @defect INT
	DECLARE @SupplierCountName NVARCHAR(MAX)
	DECLARE @SupplierCountNameA NVARCHAR(MAX)
	DECLARE @SupplierCountNameNew NVARCHAR(MAX)

SET @COUNT = 1

WHILE (@Count < 13)
BEGIN
   SET @venderCode=(SELECT Vender from #xnewReference where Id=@COUNT)

   --select @venderCode


SET @SupplierCountNameA =
(SELECT STRING_agg(cast(MV as nvarchar(max)),',') from  #xyNewA where #xyNewA.MV --20
IN (SELECT value from string_split(@venderCode,',')))
--(SELECT STRING_agg(CAST(Material AS VARCHAR(MAX)), ',') from  #xyNewA where #xyNewA.Material --20
--IN (SELECT value from string_split(@venderCode,','))) 


--select @SupplierCountNameA

SET @SupplierCountNameNew=
--(SELECT STRING_AGG(s.value, ',') 
--FROM string_split(@venderCode, ',') AS s
--LEFT JOIN string_split(@SupplierCountNameA, ',') AS s2 ON s.value = s2.value
--WHERE s2.value IS NULL)

(select STRING_agg(value,',') from string_split(@venderCode,',') 
where value NOT IN(SELECT VALUE FROM string_split(@SupplierCountNameA,',')))

--select @SupplierCountNameNew

  Insert into #TempMain
SELECT  id,MTBP_Range,Mtbp,Material as vender,defect,Vender2 from (
SELECT  @COUNT as id, MTBP_Range,mtbp,Material,defect, Vender as Vender2 from  #xyNewA where #xyNewA.MV 
--IN (SELECT Material from #x)) X
IN (SELECT value from string_split(@venderCode,',')))x ---8 vender


UNION
    SELECT
        @COUNT,
        'No Supply',
        0,
        value,
        0,
		value
    FROM STRING_SPLIT(@SupplierCountNameNew, ',')

    SET @Count = @Count + 1
END


--SELECT * from #TempMain

  SELECT * from #TempMain 
  where (@header = 'Total' or Reference_Period=@header) and ID=@Id 
  order by MTBP DESC

 
--select getdate()


 drop table #xy
 drop table #yx
  drop table #x
 drop table #y
 
 drop table #xNew
--drop table #xyNew
drop table #xyNewA
drop table #xnewReference
drop table #TempMain
drop table #TempTableN
drop table #TempTableNA
--drop table #XNewMain


END
GO
