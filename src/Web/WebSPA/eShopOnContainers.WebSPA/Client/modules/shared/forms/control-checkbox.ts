import { ControlBase } from './control-base';

export class ControlCheckbox extends ControlBase<string> {
    type: string;

    constructor(options: any = {}) {
        super(options);
        this.type = 'checkbox';
        this.value = options.value || false;
    }
}
