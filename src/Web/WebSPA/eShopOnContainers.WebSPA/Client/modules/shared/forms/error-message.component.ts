import { Component, Host, Input } from '@angular/core';
import { FormGroupDirective } from '@angular/forms';

import { ControlBase } from './control-base';
import { ValidationService } from './validation.service';

@Component({
    selector: 'appc-control-error-message',
    template: `<div *ngIf="errorMessage" class="form-control-feedback"> {{errorMessage}} </div>`
})
export class ErrorMessageComponent {
    @Input() control: ControlBase<any>;
    @Input() form: FormGroupDirective;
    constructor() { }

    get errorMessage() {
        let c = this.form.form.get(this.control.key);
        for (let propertyName in c.errors) {
            if (c.errors.hasOwnProperty(propertyName) && c.touched) {
                return ValidationService.getValidatorErrorMessage(propertyName, this.control.minlength || this.control.maxlength);
            }
        }
        return undefined;
    }
}
