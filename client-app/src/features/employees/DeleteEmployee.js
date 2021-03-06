import React, { Fragment } from 'react';
import { Button, Header, Icon } from 'semantic-ui-react';
import { User } from '../../app/api/agent';
import { history } from '../..';

import { useModalDispatch } from '../../app/context/modal/modalContext';
import { useAlertDispatch } from '../../app/context/alerts/alertContext';
import { setAlert } from '../../app/context/alerts/alertActions';

//modal to delete employee
const DeleteEmployee = ({ username }) => {
  const modalDispatch = useModalDispatch();
  const alertDispatch = useAlertDispatch();

  const onDelete = () => {
    User.deleteUser(username).then(() => {
      modalDispatch({ type: 'CLOSE_MODAL' });
      history.push('/employees');
      setAlert(alertDispatch, `Employee ${username} deleted`, 'error');
    });
  };

  return (
    <Fragment>
      <div style={{ display: 'grid' }}>
        <Header icon style={{ margin: 'auto', marginBottom: '5px' }}>
          <Icon name='user x' />
          Delete User ?
        </Header>
        <p style={{ fontStyle: 'italic', color: 'red' }}>
          Warning: Deletion will include this user's timestamps and jobsite
          history.
        </p>
        <Button
          color='red'
          inverted
          onClick={() => modalDispatch({ type: 'CLOSE_MODAL' })}
          style={{ marginBottom: '5px' }}
        >
          <Icon name='remove' /> No
        </Button>
        <Button
          color='green'
          inverted
          onClick={onDelete}
          style={{ marginBottom: '5px' }}
        >
          <Icon name='checkmark' /> Yes
        </Button>
      </div>
    </Fragment>
  );
};

export default DeleteEmployee;
