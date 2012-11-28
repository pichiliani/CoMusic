package org.herac.tuxguitar.gui.tools.collab;

import org.eclipse.swt.SWT;
import org.eclipse.swt.events.SelectionAdapter;
import org.eclipse.swt.events.SelectionEvent;
import org.eclipse.swt.layout.GridData;
import org.eclipse.swt.layout.GridLayout;
import org.eclipse.swt.widgets.Button;
import org.eclipse.swt.widgets.Composite;
import org.eclipse.swt.widgets.Label;
import org.eclipse.swt.widgets.List;
import org.eclipse.swt.widgets.Shell;
import org.eclipse.swt.widgets.Text;
import org.herac.tuxguitar.gui.TuxGuitar;
import org.herac.tuxguitar.gui.util.DialogUtils;
import org.herac.tuxguitar.gui.tools.collab.SessionDialog;

public class CollabDialog {
	
	public void show() {
		
		final Shell dialog = DialogUtils.newDialog(TuxGuitar.instance().getShell(), SWT.DIALOG_TRIM | SWT.APPLICATION_MODAL);
		dialog.setLayout(new GridLayout());
		dialog.setText("Connect to the Music Server");
		dialog.setLayoutData(new GridData(SWT.FILL,SWT.FILL,true,true));
		
		// ----------------------------------------------------------------------
		
		Composite composite = new Composite(dialog, SWT.NONE);
		composite.setLayout(new GridLayout(2,false));
		
		//-------SERVER------------------------------------
		Label labelServer = new Label(composite,SWT.LEFT);
		labelServer.setLayoutData(new GridData(SWT.LEFT,SWT.TOP,false,true));
		labelServer.setText("Server:");

		final Text TxtServer = new Text(composite,SWT.BORDER | SWT.LEFT | SWT.WRAP);
		TxtServer.setLayoutData(new GridData(300,SWT.DEFAULT));
		TxtServer.setText("192.168.1.32");
		
		//-------TCP PORT------------------------------------
		
		Label labelPort = new Label(composite,SWT.LEFT);
		labelPort.setLayoutData(new GridData(SWT.LEFT,SWT.TOP,false,true));
		labelPort.setText("TCP Port:");

		final Text TxtPort = new Text(composite,SWT.BORDER | SWT.LEFT | SWT.WRAP);
		TxtPort.setLayoutData(new GridData(300,SWT.DEFAULT));
		TxtPort.setText("100");
		
		//-------USER------------------------------------
		
		Label labelUser = new Label(composite,SWT.LEFT);
		labelUser.setLayoutData(new GridData(SWT.LEFT,SWT.TOP,false,true));
		labelUser.setText("User:");
		
		final  Text TxtUser = new Text(composite,SWT.BORDER | SWT.LEFT | SWT.WRAP);
		TxtUser.setLayoutData(new GridData(300,SWT.DEFAULT));
		TxtUser.setText("C");

		//-------PASSWORD------------------------------------
		
		Label labelPass = new Label(composite,SWT.LEFT);
		labelPass.setLayoutData(new GridData(SWT.LEFT,SWT.TOP,false,true));
		labelPass.setText("Password:");
		
		final  Text TxtPassword = new Text(composite,SWT.BORDER | SWT.LEFT | SWT.WRAP);
		TxtPassword.setLayoutData(new GridData(300,SWT.DEFAULT));
		TxtPassword.setText("C");

		//------------------BUTTONS--------------------------
		Composite buttons = new Composite(dialog, SWT.NONE);
		buttons.setLayout(new GridLayout(2,false));
		buttons.setLayoutData(new GridData(SWT.END,SWT.FILL,true,true));
		
		final Button buttonOK = new Button(buttons, SWT.PUSH);
		buttonOK.setText("Connect");
		buttonOK.setLayoutData(getButtonData());
		buttonOK.addSelectionListener(new SelectionAdapter() {
			public void widgetSelected(SelectionEvent arg0) 
			{
			    
				// Primeiro colocando a mensagem de conexão.
				// ProjectBrowser.getInstance().getStatusBar().showStatus("Conectando ArgoUML...");
	
				// Criar a Thread que vai enviar os eventos (colocar esta prog. no main ou como um plug in)
				if ( TuxGuitar.instance().clienteEnvia.SetaConecta(TxtServer.getText() 
						                                            ,Integer.valueOf(TxtPort.getText()).intValue() ) )
				{
	
					// Agora vou verificar se o login e a senha estão corretos
	
					if( TuxGuitar.instance().clienteEnvia.SetaUser(TxtUser.getText(),TxtPassword.getText() ) )
					{
					
						TuxGuitar.instance().clienteEnvia.start();
					     
						// ProjectBrowser.getInstance().getStatusBar().showStatus("Conectado");
						
						// Fechando a janela atual
						dialog.dispose();
						
						// Aqui vou iniciar a nova janela com as informações para conexão
						new SessionDialog().show();
						
						
					}
				}
				
				
				// TuxGuitar.instance().getScaleManager().selectScale((scales.getSelectionIndex() - 1), keys.getSelectionIndex());
				dialog.dispose();
			}
		});
		
		Button buttonCancel = new Button(buttons, SWT.PUSH);
		buttonCancel.setText(TuxGuitar.getProperty("cancel"));
		buttonCancel.setLayoutData(getButtonData());
		buttonCancel.addSelectionListener(new SelectionAdapter() {
			public void widgetSelected(SelectionEvent arg0) {
				dialog.dispose();
			}
		});
		
		dialog.setDefaultButton( buttonOK );
		
		DialogUtils.openDialog(dialog,DialogUtils.OPEN_STYLE_CENTER | DialogUtils.OPEN_STYLE_PACK | DialogUtils.OPEN_STYLE_WAIT);
	}
	
	private GridData getButtonData(){
		GridData data = new GridData(SWT.FILL, SWT.FILL, true, true);
		data.minimumWidth = 80;
		data.minimumHeight = 25;
		return data;
	}
}
