<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:template match="/">
<html>
	<head>
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
		<title>Manager</title>
		<style type="text/css">
            body {
            font-family:Verdana, Arial, Helvetica, sans-serif;
            font-size:100%;
            }
            h1 {font-size:1.250em;}
            h2 {font-size:1.125em;}
            h3 {font-size:1.000em;}
            p {font-size:0.750em;}
            table {
            background-color:white;
            color:black;
            border-collapse:collapse;
            font-size:0.750em;
            }
            table, th, td {
            border: 1px solid blue;
            }
            th.sep, td.sep {
            background-color:white;
            color:black;
            border: 0px;
            }
            th, td {
            padding-left:8px;
            padding-right:8px;
            }
            th {
            background-color:blue;
            color:white;
            }
            .mysmall {font-size:0.750em;}
            .mysmallest {font-size:0.500em;}
        </style>
	</head>

	<body>
	<h1>#Title#</h1>
    <div class="mysmallest">(<xsl:value-of select="JBCConfigLog/@Date"/>&#160;<xsl:value-of select="JBCConfigLog/@Time"/>)</div> 
	
    <xsl:if test="JBCConfigLog/Comment">
	    <h2><xsl:value-of select="JBCConfigLog/Comment/@Text"/>: <xsl:value-of select="JBCConfigLog/Comment"/></h2>
    </xsl:if>

	<xsl:for-each select="JBCConfigLog/SourceParams">
        
		<h2><xsl:value-of select="./@Text"/> (<xsl:value-of select="./@TempUnits"/>)</h2>
		<table border="0">
		<tr>
		  <th>#Parameter#</th>
		  <th>#Value#</th>
		  <th class="sep"></th>
		  <th>#Parameter#</th>
		  <th>#Value#</th>
		</tr>
		<xsl:for-each select="./StationSettings/*">
		  <tr>
		  <xsl:choose>
			<xsl:when test="position() mod 2 != 0">
				<td><xsl:value-of select="@Text"/></td>
				<td><xsl:value-of select="."/></td>
				<td class="sep"></td>
				<td><xsl:value-of select="following-sibling::node()/@Text"/></td>
				<td><xsl:value-of select="following-sibling::node()"/></td>
			</xsl:when>
		  </xsl:choose>
		  </tr>
		</xsl:for-each>
		</table>
		<p />
		<table border="0">
		<tr>
		  <th><xsl:value-of select="./PortsAndTools/Port[@Number='1']/@Text"/></th>
		  <th><xsl:value-of select="./PortsAndTools/Port[@Number='1']/Tool[1]/@Text"/></th>
		  <th><xsl:value-of select="./PortsAndTools/Port[@Number='1']/toolSelectedTemp/@Text"/></th>
		  <xsl:for-each select="./PortsAndTools/Port[@Number='1']/Tool[1]/*">
			<th><xsl:value-of select="@Text"/></th>
		  </xsl:for-each>
		</tr>
		<xsl:for-each select="./PortsAndTools/*">
			<tr>
			<td><xsl:value-of select="@Number"/></td>
			<td> </td>
			<td><xsl:value-of select="./toolSelectedTemp"/></td>
			</tr>
			<xsl:for-each select="./Tool">
			<tr>
				<td> </td>
				<td><xsl:value-of select="@Type"/></td>
				<td> </td>
				<xsl:for-each select="./*">
					<td style="white-space: nowrap"><xsl:value-of select="."/>
						<xsl:if test="@Enabled">
							<xsl:choose>
								<xsl:when test="./@Enabled = 'True'">
								(#_On#)
								</xsl:when>
								<xsl:otherwise>
								(#_Off#)
								</xsl:otherwise>
							</xsl:choose>
						</xsl:if>
					</td>
				</xsl:for-each>
			</tr>
			</xsl:for-each>
		</xsl:for-each>
		</table>
	
    </xsl:for-each>

	<xsl:for-each select="JBCConfigLog/TargetStations">
		<h2><xsl:value-of select="./@Text"/></h2>
		<xsl:for-each select="./Station">
			<h3><xsl:value-of select="./@Text"/>: <xsl:value-of select="./@stnName"/> - <xsl:value-of select="./@stnModel"/></h3>		
			<table border="0">
			<xsl:for-each select="./Ports/*">
				<tr>
				<td><xsl:value-of select="./@TextFromPort"/></td>
				</tr>
			</xsl:for-each>
			</table>
			<table border="0">
			<tr>
			<td><xsl:value-of select="./Logs/@Text"/></td>
			</tr>
			<xsl:for-each select="./Logs/*">
				<tr>
				<td><xsl:value-of select="./@Text"/> <xsl:value-of select="."/></td>
				</tr>
			</xsl:for-each>
			</table>
		</xsl:for-each>
    </xsl:for-each>
	
	</body>
</html>
</xsl:template>

</xsl:stylesheet>
