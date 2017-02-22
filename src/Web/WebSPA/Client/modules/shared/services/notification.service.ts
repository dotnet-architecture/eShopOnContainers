import { Injectable } from '@angular/core';

@Injectable()
export class NotificationService {

    printSuccessMessage(message: string) {
        console.log(message);
    }

    printErrorMessage(message: string) {
        console.error(message);
    }
}
