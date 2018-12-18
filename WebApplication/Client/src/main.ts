import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';

if (environment.production) {
  enableProdMode();
}

platformBrowserDynamic().bootstrapModule(AppModule)
  .catch(err => console.log(err));


/**
 * Storage prefix
 */
const storagePrefix = 'fotron_';

let originStorageSetItem = Storage.prototype.setItem;
Storage.prototype.setItem = function(key, value) {
  return originStorageSetItem.call(this, storagePrefix + key, value);
}

let originStorageGetItem = Storage.prototype.getItem;
Storage.prototype.getItem = function(key) {
  return originStorageGetItem.call(this,storagePrefix + key);
}

let originStorageRemoveItem = Storage.prototype.removeItem;
Storage.prototype.removeItem = function(key) {
  return originStorageRemoveItem.call(this, storagePrefix + key);
}
