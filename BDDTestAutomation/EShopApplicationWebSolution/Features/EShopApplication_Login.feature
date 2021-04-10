@ui @owner=kritsha @web @testplan= @testsuite=
Feature: Login_EShopApplication
	As a user, I want to be able to login to the EShop application

@bvt @priority=1
Scenario: Verify that a registered user is able to login into EShop application
	When user launches EShop application
	And user clicks on "Login" option
	And user enter Email and Password of "User1"
	And user click on "LOG IN" button
	Then verify if "User1" is logged-in
	
@priority=2
Scenario Outline: Verify that the user is unable to login to EShop Application if user is not registered already
	Given the user is not registered to EShop application
	When user launches EShop application
	And user clicks on "Login" button 
	And user enter Email and Password of "<user>"
	And user click on "LOG IN" button
	Then the user should not be able to login to the application
	Examples: 
	| user         |
	| InvalidUser1 |
	| InvalidUser2 |
	| InvalidUser3 |
