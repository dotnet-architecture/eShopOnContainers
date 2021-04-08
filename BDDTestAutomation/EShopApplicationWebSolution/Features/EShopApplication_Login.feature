@ui @owner=kritsha @web @testplan= @testsuite=
Feature: Login_EShopApplication
	As a user, I want to be able to login to the EShop application

@bvt @priority=1
Scenario: Verify that a registered user is able to login into EShop application
	Given the user is registered to EShop application
	When user launches EShop application
	And user clicks on "Login" button 
	And user enters "email" and "password"
	When user clicks on "Log in"
	Then the user should be able to login to the application
	
@priority=2
Scenario Outline: Verify that the user is unable to login to EShop Application if user is not registered already
	Given the user is no registered to EShop application
	When user launches EShop application
	And user clicks on "Login" button 
	And user enters "email" and "password"
	When user clicks on "Log in"
	Then the user should not be able to login to the application

	Examples: 
	| username | password  |
	| user1    | password1 |
	| user2    | password2 |
	| user3    | password3 |
