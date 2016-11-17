import { ControlBase } from './control-base';

export class ControlTextbox extends ControlBase<boolean> {
    constructor(options: any = {}) {
        super(options);
        this.type = options.type || 'textbox';
    }
}
