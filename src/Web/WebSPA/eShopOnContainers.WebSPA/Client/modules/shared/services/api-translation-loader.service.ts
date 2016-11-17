import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Rx';
import { TranslateLoader } from 'ng2-translate/ng2-translate';
import { MissingTranslationHandler, MissingTranslationHandlerParams } from 'ng2-translate/ng2-translate';

import { ContentService } from './content.service';

@Injectable()
export class ApiTranslationLoader implements TranslateLoader {

    constructor(private cs: ContentService) { }

    getTranslation(lang: string): Observable<any> {
        return this.cs.get(lang);
    }
}

@Injectable()
export class CustomMissingTranslationHandler implements MissingTranslationHandler {
    handle(params: MissingTranslationHandlerParams) {
        return params.key;
    }
}
