import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { AppComponent } from './app.component';
import { TaskService } from './services/task.service';

describe('AppComponent', () => {
  const taskServiceMock: Pick<TaskService, 'getTasks' | 'markAsDone'> = {
    getTasks: () => of([]),
    markAsDone: () => of({
      id: 'id',
      title: 'title',
      description: '',
      createdDate: new Date().toISOString(),
      status: 'Done'
    })
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppComponent],
      providers: [{ provide: TaskService, useValue: taskServiceMock }]
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render title', () => {
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('Task Manager');
  });
});
