call "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\Tools\VsDevCmd.bat"
xsd sch.xsd /t:c /c /n:ExcelLoading
copy sch.xsd xmlexport\sch.xsd /Y
copy sch.xsd ColonyRuler\Assets\Scripts\XMLScripts\sch.xsd /y