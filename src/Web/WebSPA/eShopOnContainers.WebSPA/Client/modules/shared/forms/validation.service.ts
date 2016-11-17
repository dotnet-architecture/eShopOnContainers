export class ValidationService {

    static getValidatorErrorMessage(code: string, fieldLength: number) {
        let config: any = {
            'required': 'This is a required field',
            'minlength': 'Minimum length is ' + fieldLength,
            'maxlength': 'Maximum length is ' + fieldLength,
            'invalidCreditCard': 'Invalid credit card number',
            'invalidEmailAddress': 'Invalid email address',
            'invalidPassword': 'Password must be at least 6 characters long, and contain a number and special character.'
        };
        return config[code];
    }

    static creditCardValidator(control: any) {
        // Visa, MasterCard, American Express, Diners Club, Discover, JCB
        if (control.value.match(/^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$/)) {
            return undefined;
        } else {
            return { 'invalidCreditCard': true };
        }
    }

    static emailValidator(control: any) {
        // RFC 2822 compliant regex
        if (control.value.match(/[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?/)) {
            return undefined;
        } else {
            return { 'invalidEmailAddress': true };
        }
    }

    static passwordValidator(control: any) {
        // {6,100}           - Assert password is between 6 and 100 characters
        // (?=.*[0-9])       - Assert a string has at least one number
        if (control.value.match(/^(?=.*[0-9])[a-zA-Z0-9!"@#$%^&*]{6,100}$/)) {
            return undefined;
        } else {
            return { 'invalidPassword': true };
        }
    }
}
