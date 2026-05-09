import { CommonModule } from '@angular/common';
import {
  AfterViewInit,
  Component,
  ElementRef,
  Input,
  OnDestroy,
  signal,
  ViewChild,
} from '@angular/core';

@Component({
  selector: 'app-sticky-buttons-container',
  imports: [CommonModule],
  templateUrl: './sticky-buttons-container.html',
  styleUrl: './sticky-buttons-container.scss',
})
export class StickyButtonsContainer implements AfterViewInit, OnDestroy {
  @ViewChild('buttonsContainer') buttonsContainer!: ElementRef;

  @Input()
  fullWidth = false;

  fromModal = signal(false);

  observer?: IntersectionObserver;

  ngAfterViewInit(): void {
    const modalContainer = this.buttonsContainer.nativeElement.closest(
      '.dialog-with-sticky-buttons',
    );
    this.fromModal.set(!!modalContainer);

    this.observer = new IntersectionObserver(
      ([entry]) => {
        if (entry.intersectionRatio < 1) {
          entry.target.classList.add('sticky');
        } else {
          entry.target.classList.remove('stuck');
        }
      },
      { threshold: [1], root: modalContainer },
    );

    this.observer.observe(this.buttonsContainer.nativeElement);
  }

  ngOnDestroy(): void {
    this.observer?.disconnect();
  }
}
