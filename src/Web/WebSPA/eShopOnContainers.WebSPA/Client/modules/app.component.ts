import { Title } from '@angular/platform-browser';
import { Component, ViewEncapsulation, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TranslateService } from 'ng2-translate/ng2-translate';

import { DataService } from './shared/services/data.service';

/*
 * App Component
 * Top Level Component
 */

@Component({
  selector: 'appc-app',
  styleUrls: ['./app.component.scss'],
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {


  constructor(private translate: TranslateService, private titleService: Title) {
    // this language will be used as a fallback when a translation isn't found in the current language
    translate.setDefaultLang('en');

    // the lang to use, if the lang isn't available, it will use the current loader to get them
    translate.use('en');
  }

  ngOnInit() {
    this.translate.get('title')
      .subscribe(title => this.setTitle(title));
  }

  public setTitle(newTitle: string) {
    this.titleService.setTitle(newTitle);
  }
}
