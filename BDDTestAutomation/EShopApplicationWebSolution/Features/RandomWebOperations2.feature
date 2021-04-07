@ui @owner=vamsitp @web
Feature: RandomWebOperations2
	In order to be efficient at Testing
	As a Test enthusiast
	I want to be able to know more about BDD

#Sample to invoke a runtime-method dynamically before or after executing a scenario
@before=CreateDataFile(SomeData) @after=DeleteDataFile()
Scenario: Verify File-upload
	Given I have launched "FileUpload" site
	And I have clicked "browse" button
	When I enter path of a file to be uploaded
	Then the value should be set
	And I verify if data in the file is "SomeData"

@priority=1
Scenario: Verify File-download
	Given I have launched "FileDownload" site
	And I have clicked "download" button
	When I search the downloads tab for the "Sample.xlsx" file
	Then the content of the file should be valid
